using System.Linq.Expressions;
using System.Reflection;
using HomePal.Domain.Common;
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
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Pantry> Pantries => Set<Pantry>();
    public DbSet<PantryItem> PantryItems => Set<PantryItem>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<HouseholdMonthlyBudget> HouseholdMonthlyBudgets => Set<HouseholdMonthlyBudget>();
    public DbSet<HouseholdExpense> HouseholdExpenses => Set<HouseholdExpense>();
    public DbSet<AgentChatSession> AgentChatSessions => Set<AgentChatSession>();
    public DbSet<AgentChatMessage> AgentChatMessages => Set<AgentChatMessage>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global soft-delete query filter to all BaseAuditableEntity types
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
                var notDeleted = Expression.Not(property);
                var lambda = Expression.Lambda(notDeleted, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDeleteInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDeleteInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditAndSoftDeleteInfo()
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
