using HomePal.Application.Common.Interfaces;
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
using HomePal.Persistence.Context;
using HomePal.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomePal.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;
    private IHouseholdRepository? _households;
    private IHouseholdMemberRepository? _householdMembers;
    private IHouseholdInvitationRepository? _householdInvitations;
    private IPreferenceRepository? _preferences;
    private IPreferenceCategoryRepository? _preferenceCategories;
    private IProductCategoryRepository? _productCategories;
    private IMeasuringUnitRepository? _measuringUnits;
    private ISupermarketRepository? _supermarkets;
    private IOfferRepository? _offers;
    private IPantryRepository? _pantries;
    private IPantryItemRepository? _pantryItems;
    private IShoppingListRepository? _shoppingLists;
    private IShoppingListItemRepository? _shoppingListItems;
    private IHouseholdMonthlyBudgetRepository? _monthlyBudgets;
    private IHouseholdExpenseRepository? _householdExpenses;
    private IAgentChatRepository? _agentChats;
    private IMealPlanRepository? _mealPlans;
    private IHouseholdReportRepository? _reports;
    private IAdminAnalyticsRepository? _adminAnalytics;
    private IGovernorateRepository? _governorates;
    private ICityRepository? _cities;
    private ISubscriptionPlanRepository? _subscriptionPlans;
    private IUserSubscriptionRepository? _userSubscriptions;
    private IPaymentTransactionRepository? _paymentTransactions;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public IHouseholdRepository Households => _households ??= new HouseholdRepository(_context);
    public IHouseholdMemberRepository HouseholdMembers => _householdMembers ??= new HouseholdMemberRepository(_context);
    public IHouseholdInvitationRepository HouseholdInvitations => _householdInvitations ??= new HouseholdInvitationRepository(_context);
    public IPreferenceRepository Preferences => _preferences ??= new PreferenceRepository(_context);
    public IPreferenceCategoryRepository PreferenceCategories => _preferenceCategories ??= new PreferenceCategoryRepository(_context);
    public IProductCategoryRepository ProductCategories => _productCategories ??= new ProductCategoryRepository(_context);
    public IMeasuringUnitRepository MeasuringUnits => _measuringUnits ??= new MeasuringUnitRepository(_context);
    public ISupermarketRepository Supermarkets => _supermarkets ??= new SupermarketRepository(_context);
    public IOfferRepository Offers => _offers ??= new OfferRepository(_context);
    public IPantryRepository Pantries => _pantries ??= new PantryRepository(_context);
    public IPantryItemRepository PantryItems => _pantryItems ??= new PantryItemRepository(_context);
    public IShoppingListRepository ShoppingLists => _shoppingLists ??= new ShoppingListRepository(_context);
    public IShoppingListItemRepository ShoppingListItems => _shoppingListItems ??= new ShoppingListItemRepository(_context);
    public IHouseholdMonthlyBudgetRepository MonthlyBudgets => _monthlyBudgets ??= new HouseholdMonthlyBudgetRepository(_context);
    public IHouseholdExpenseRepository HouseholdExpenses => _householdExpenses ??= new HouseholdExpenseRepository(_context);
    public IAgentChatRepository AgentChats => _agentChats ??= new AgentChatRepository(_context);
    public IMealPlanRepository MealPlans => _mealPlans ??= new MealPlanRepository(_context);
    public IHouseholdReportRepository Reports => _reports ??= new HouseholdReportRepository(_context);
    public IAdminAnalyticsRepository AdminAnalytics => _adminAnalytics ??= new AdminAnalyticsRepository(_context);
    public IGovernorateRepository Governorates => _governorates ??= new GovernorateRepository(_context);
    public ICityRepository Cities => _cities ??= new CityRepository(_context);
    public ISubscriptionPlanRepository SubscriptionPlans => _subscriptionPlans ??= new SubscriptionPlanRepository(_context);
    public IUserSubscriptionRepository UserSubscriptions => _userSubscriptions ??= new UserSubscriptionRepository(_context);
    public IPaymentTransactionRepository PaymentTransactions => _paymentTransactions ??= new PaymentTransactionRepository(_context);


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
