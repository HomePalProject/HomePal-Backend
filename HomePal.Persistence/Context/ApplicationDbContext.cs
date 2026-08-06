using System.Reflection;
using HomePal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();
    public DbSet<HouseholdInvitation> HouseholdInvitations => Set<HouseholdInvitation>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<PreferenceCategory> PreferenceCategories => Set<PreferenceCategory>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<MeasuringUnit> MeasuringUnits => Set<MeasuringUnit>();
    public DbSet<Supermarket> Supermarkets => Set<Supermarket>();
    public DbSet<CanonicalProduct> CanonicalProducts => Set<CanonicalProduct>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Pantry> Pantries => Set<Pantry>();
    public DbSet<PantryItem> PantryItems => Set<PantryItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
