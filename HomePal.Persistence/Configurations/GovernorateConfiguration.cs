using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
{
    public void Configure(EntityTypeBuilder<Governorate> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(g => g.Code)
            .IsUnique();

        builder.OwnsMany(g => g.Name, n => n.ToJson());

        builder.HasMany(g => g.Cities)
            .WithOne(c => c.Governorate)
            .HasForeignKey(c => c.GovernorateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
