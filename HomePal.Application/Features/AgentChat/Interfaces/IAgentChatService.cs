using System.Net.ServerSentEvents;
using HomePal.Application.Features.AgentChat.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.AgentChat.Interfaces;

public interface IAgentChatService
{
    Task<Result<AgentChatSessionResponse>> GetUserSessionAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<object>> ClearUserSessionAsync(Guid userId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<SseItem<string>> RunAgentStreamAsync(Guid userId, RunAgentRequest request, CancellationToken cancellationToken = default);
}
