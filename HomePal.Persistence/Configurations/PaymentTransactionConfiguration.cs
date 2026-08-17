using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.PaymobOrderId);
        builder.HasIndex(t => t.PaymobTransactionId);

        builder.Property(t => t.PaymobOrderId)
            .HasMaxLength(100);

        builder.Property(t => t.PaymobTransactionId)
            .HasMaxLength(100);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.PaymentMethod)
            .HasMaxLength(50);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(t => t.Subscription)
            .WithMany(s => s.PaymentTransactions)
            .HasForeignKey(t => t.SubscriptionId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(t => t.Plan)
            .WithMany(p => p.PaymentTransactions)
            .HasForeignKey(t => t.PlanId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
