using HomePal.Application.Features.Subscriptions.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaymentTransaction?> GetByPaymobOrderIdAsync(string paymobOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Plan)
            .Include(t => t.Subscription)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.PaymobOrderId == paymobOrderId, cancellationToken);
    }

    public async Task<PaymentTransaction?> GetByPaymobTransactionIdAsync(string paymobTransactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Plan)
            .Include(t => t.Subscription)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.PaymobTransactionId == paymobTransactionId, cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.Plan)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
