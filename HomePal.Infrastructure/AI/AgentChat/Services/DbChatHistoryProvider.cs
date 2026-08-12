using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using AiMessage = Microsoft.Extensions.AI.ChatMessage;

namespace HomePal.Infrastructure.AI.AgentChat.Services;

/// <summary>
/// Native ChatHistoryProvider implementation for Microsoft Agent Framework in HomePal.
/// Persists single-user conversation history using directly injected IUnitOfWork.
/// </summary>
public class DbChatHistoryProvider : ChatHistoryProvider
{
    private readonly IUnitOfWork _unitOfWork;

    public DbChatHistoryProvider(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async ValueTask<IEnumerable<AiMessage>> ProvideChatHistoryAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.Session?.StateBag != null &&
            context.Session.StateBag.TryGetValue<string>("ChatSessionId", out var chatIdStr) &&
            Guid.TryParse(chatIdStr, out var chatSessionId))
        {
            var session = await _unitOfWork.AgentChats.GetByIdAsync(chatSessionId, cancellationToken);
            if (session != null)
            {
                return session.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new AiMessage(
                        m.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                        m.Content));
            }
        }

        return Array.Empty<AiMessage>();
    }

    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.Session?.StateBag != null &&
            context.Session.StateBag.TryGetValue<string>("ChatSessionId", out var chatIdStr) &&
            Guid.TryParse(chatIdStr, out var chatSessionId))
        {
            var session = await _unitOfWork.AgentChats.GetByIdAsync(chatSessionId, cancellationToken);
            if (session == null)
            {
                return;
            }

            // Store user request messages
            if (context.RequestMessages != null)
            {
                foreach (var message in context.RequestMessages)
                {
                    if (!string.IsNullOrWhiteSpace(message.Text))
                    {
                        await _unitOfWork.AgentChats.AddMessageAsync(new AgentChatMessage
                        {
                            Id = Guid.NewGuid(),
                            ChatSessionId = chatSessionId,
                            Role = message.Role == ChatRole.User ? "user" : "assistant",
                            Content = message.Text,
                            CreatedAt = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
            }

            // Store assistant response messages
            if (context.ResponseMessages != null)
            {
                foreach (var message in context.ResponseMessages)
                {
                    if (!string.IsNullOrWhiteSpace(message.Text))
                    {
                        await _unitOfWork.AgentChats.AddMessageAsync(new AgentChatMessage
                        {
                            Id = Guid.NewGuid(),
                            ChatSessionId = chatSessionId,
                            Role = message.Role == ChatRole.User ? "user" : "assistant",
                            Content = message.Text,
                            CreatedAt = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
            }

            // Persist session state serialization payload
            if (context.Agent != null && context.Session != null)
            {
                var sessionJson = await context.Agent.SerializeSessionAsync(context.Session, cancellationToken: cancellationToken);
                session.SessionStateJson = sessionJson.GetRawText();
                session.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
