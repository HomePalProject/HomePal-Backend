using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
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
    private ICanonicalProductRepository? _canonicalProducts;
    private IOfferRepository? _offers;
    private IPantryRepository? _pantries;
    private IPantryItemRepository? _pantryItems;

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
    public ICanonicalProductRepository CanonicalProducts => _canonicalProducts ??= new CanonicalProductRepository(_context);
    public IOfferRepository Offers => _offers ??= new OfferRepository(_context);
    public IPantryRepository Pantries => _pantries ??= new PantryRepository(_context);
    public IPantryItemRepository PantryItems => _pantryItems ??= new PantryItemRepository(_context);

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
