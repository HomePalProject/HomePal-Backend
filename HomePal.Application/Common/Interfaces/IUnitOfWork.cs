using HomePal.Application.Features.AgentChat.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.Locations.Interfaces;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Application.Features.Subscriptions.Interfaces;

namespace HomePal.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IHouseholdRepository Households { get; }
    IHouseholdMemberRepository HouseholdMembers { get; }
    IHouseholdInvitationRepository HouseholdInvitations { get; }
    IPreferenceRepository Preferences { get; }
    IPreferenceCategoryRepository PreferenceCategories { get; }
    IProductCategoryRepository ProductCategories { get; }
    IMeasuringUnitRepository MeasuringUnits { get; }
    ISupermarketRepository Supermarkets { get; }
    IOfferRepository Offers { get; }
    IPantryRepository Pantries { get; }
    IPantryItemRepository PantryItems { get; }
    IShoppingListRepository ShoppingLists { get; }
    IShoppingListItemRepository ShoppingListItems { get; }
    IHouseholdMonthlyBudgetRepository MonthlyBudgets { get; }
    IHouseholdExpenseRepository HouseholdExpenses { get; }
    IAgentChatRepository AgentChats { get; }
    IMealPlanRepository MealPlans { get; }
    IHouseholdReportRepository Reports { get; }
    IAdminAnalyticsRepository AdminAnalytics { get; }
    IGovernorateRepository Governorates { get; }
    ICityRepository Cities { get; }
    ISubscriptionPlanRepository SubscriptionPlans { get; }
    IUserSubscriptionRepository UserSubscriptions { get; }
    IPaymentTransactionRepository PaymentTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
