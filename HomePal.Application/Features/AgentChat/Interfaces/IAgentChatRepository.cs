using HomePal.Domain.Entities;

namespace HomePal.Application.Features.AgentChat.Interfaces;

public interface IAgentChatRepository
{
    Task<AgentChatSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgentChatSession?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AgentChatSession> GetOrCreateByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddMessageAsync(AgentChatMessage message, CancellationToken cancellationToken = default);
    Task ClearSessionAsync(Guid userId, CancellationToken cancellationToken = default);
}
