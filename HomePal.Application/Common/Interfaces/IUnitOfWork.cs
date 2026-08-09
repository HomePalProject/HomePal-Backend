using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;

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
    HomePal.Application.Features.ShoppingList.Interfaces.IShoppingListRepository ShoppingLists { get; }
    HomePal.Application.Features.ShoppingList.Interfaces.IShoppingListItemRepository ShoppingListItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
