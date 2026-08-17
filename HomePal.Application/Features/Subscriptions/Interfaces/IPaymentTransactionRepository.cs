using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Subscriptions.Interfaces;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetByPaymobOrderIdAsync(string paymobOrderId, CancellationToken cancellationToken = default);
    Task<PaymentTransaction?> GetByPaymobTransactionIdAsync(string paymobTransactionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
