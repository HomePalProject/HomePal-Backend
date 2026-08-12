using HomePal.Application.Features.AgentChat.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class AgentChatRepository : Repository<AgentChatSession>, IAgentChatRepository
{
    public AgentChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<AgentChatSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<AgentChatSession?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<AgentChatSession> GetOrCreateByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await GetByUserIdAsync(userId, cancellationToken);

        if (session is null)
        {
            session = new AgentChatSession
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _dbSet.AddAsync(session, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return session;
    }

    public async Task AddMessageAsync(AgentChatMessage message, CancellationToken cancellationToken = default)
    {
        await _context.AgentChatMessages.AddAsync(message, cancellationToken);
    }

    public async Task ClearSessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await GetByUserIdAsync(userId, cancellationToken);
        if (session is not null)
        {
            _context.AgentChatMessages.RemoveRange(session.Messages);
            session.SessionStateJson = null;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
