using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Subscriptions.Interfaces;

public interface IUserSubscriptionRepository : IRepository<UserSubscription>
{
    Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetLatestSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetByIdWithPlanAsync(Guid id, CancellationToken cancellationToken = default);
}
