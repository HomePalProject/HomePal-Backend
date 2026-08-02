using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.HasKey(m => m.Id);

        builder.OwnsMany(m => m.Name, n => n.ToJson());
        builder.OwnsMany(m => m.Symbol, s => s.ToJson());
        builder.OwnsMany(m => m.Description, d => d.ToJson());
    }
}
