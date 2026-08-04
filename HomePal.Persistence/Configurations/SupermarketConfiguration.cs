using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class SupermarketConfiguration : IEntityTypeConfiguration<Supermarket>
{
    public void Configure(EntityTypeBuilder<Supermarket> builder)
    {
        builder.HasKey(s => s.Id);

        builder.OwnsMany(s => s.Name, n => n.ToJson());
        builder.Property(s => s.LogoPath).HasMaxLength(500);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.WebsiteUrl).HasMaxLength(500);
    }
}
