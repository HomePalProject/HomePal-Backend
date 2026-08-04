using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class MeasuringUnitConfiguration : IEntityTypeConfiguration<MeasuringUnit>
{
    public void Configure(EntityTypeBuilder<MeasuringUnit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.OwnsMany(u => u.Name, n => n.ToJson());
        builder.OwnsMany(u => u.Symbol, n => n.ToJson());
    }
}
