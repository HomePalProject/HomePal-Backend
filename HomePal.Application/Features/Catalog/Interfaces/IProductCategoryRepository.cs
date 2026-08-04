using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<IReadOnlyList<ProductCategory>> SearchAsync(string? query, CancellationToken cancellationToken = default);
}
