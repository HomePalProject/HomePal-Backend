using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.GovernorateCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.OwnsMany(c => c.Name, n => n.ToJson());

        builder.HasIndex(c => c.GovernorateId);
        builder.HasIndex(c => c.GovernorateCode);
    }
}
