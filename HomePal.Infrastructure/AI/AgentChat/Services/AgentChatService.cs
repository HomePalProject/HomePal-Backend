using System.Net.ServerSentEvents;
using System.Text.Json;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.AgentChat.DTOs;
using HomePal.Application.Features.AgentChat.Interfaces;
using HomePal.Shared.Results;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

// Aliases
using AiMessage = Microsoft.Extensions.AI.ChatMessage;

namespace HomePal.Infrastructure.AI.AgentChat.Services;

public class AgentChatService : IAgentChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AgentSseStream _sseStream;
    private readonly AIAgent _agent;

    public AgentChatService(
        IUnitOfWork unitOfWork,
        AgentSseStream sseStream,
        [FromKeyedServices("Agent")] AIAgent agent)
    {
        _unitOfWork = unitOfWork;
        _sseStream = sseStream;
        _agent = agent;
    }

    public async Task<Result<AgentChatSessionResponse>> GetUserSessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.AgentChats.GetOrCreateByUserIdAsync(userId, cancellationToken);

        var response = new AgentChatSessionResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            Messages = session.Messages.Select(m => new AgentChatMessageResponse
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList()
        };

        return Result<AgentChatSessionResponse>.Ok(response, ErrorMessages.AgentChat.GetSessionSuccess);
    }

    public async Task<Result<object>> ClearUserSessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.AgentChats.ClearSessionAsync(userId, cancellationToken);
        return Result<object>.Ok(null!, ErrorMessages.AgentChat.ChatCleared);
    }

    public async IAsyncEnumerable<SseItem<string>> RunAgentStreamAsync(
        Guid userId,
        RunAgentRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // ── Load or create user's single chat session ──────────────────────
        var chatSession = await _unitOfWork.AgentChats.GetOrCreateByUserIdAsync(userId, cancellationToken);

        // ── Get / deserialize agent session ───────────────────────────────
        AgentSession session;
        if (!string.IsNullOrWhiteSpace(chatSession.SessionStateJson))
        {
            using var doc = JsonDocument.Parse(chatSession.SessionStateJson);
            session = await _agent.DeserializeSessionAsync(doc.RootElement, cancellationToken: cancellationToken);
        }
        else
        {
            session = await _agent.CreateSessionAsync(cancellationToken);
            session.StateBag?.SetValue("ChatSessionId", chatSession.Id.ToString());
        }

        if (session.StateBag != null && !session.StateBag.TryGetValue<string>("ChatSessionId", out _))
        {
            session.StateBag.SetValue("ChatSessionId", chatSession.Id.ToString());
        }

        var runId = Guid.NewGuid().ToString();
        var threadId = chatSession.Id.ToString();
        var messageId = Guid.NewGuid().ToString();

        IAsyncEnumerable<AgentResponseUpdate> updates;

        // ── CASE 1: Tool approval response ──────────────────────────────────
        if (request.Approved.HasValue)
        {
            if (string.IsNullOrWhiteSpace(request.ToolCallId))
            {
                yield return new SseItem<string>(JsonSerializer.Serialize(new
                {
                    type = "ERROR",
                    message = "ToolCallId is required when approving a tool call."
                }));
                yield break;
            }

            var toolCall = new FunctionCallContent(request.ToolCallId, "tool", null);
            var approvalResponse = new ToolApprovalResponseContent(
                request.ToolCallId,
                request.Approved.Value,
                toolCall);

            updates = _agent.RunStreamingAsync(
                [new AiMessage(ChatRole.User, [approvalResponse])],
                session,
                cancellationToken: cancellationToken);
        }
        // ── CASE 2: Normal user message ─────────────────────────────────────
        else
        {
            var msgText = request.Message?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(msgText))
            {
                yield return new SseItem<string>(JsonSerializer.Serialize(new
                {
                    type = "ERROR",
                    message = "Message content is required."
                }));
                yield break;
            }

            updates = _agent.RunStreamingAsync(msgText, session, cancellationToken: cancellationToken);
        }

        // ── Delegate to SSE Stream ──────────────────────────────────────────
        await foreach (var sseItem in _sseStream.RunAsync(updates, chatSession.Id, runId, threadId, messageId, cancellationToken))
        {
            yield return sseItem;
        }
    }
}
