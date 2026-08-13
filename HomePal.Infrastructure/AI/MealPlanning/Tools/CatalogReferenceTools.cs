using System.ComponentModel;
using HomePal.Application.Features.Catalog.Interfaces;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for querying catalog reference data (Product Categories & Measuring Units) via Application Services.
/// </summary>
public class CatalogReferenceTools
{
    private readonly IProductCategoryService _categoryService;
    private readonly IMeasuringUnitService _unitService;

    public CatalogReferenceTools(
        IProductCategoryService categoryService,
        IMeasuringUnitService unitService)
    {
        _categoryService = categoryService;
        _unitService = unitService;
    }

    [Description("Gets all available product categories and measuring units supported by the system with IDs, names, and symbols.")]
    public async Task<object> GetCategoriesAndUnitsAsync(CancellationToken cancellationToken = default)
    {
        var categoriesResult = await _categoryService.GetAllAsync(null, cancellationToken);
        var unitsResult = await _unitService.GetAllAsync(null, cancellationToken);

        return new
        {
            success = true,
            categories = categoriesResult.Data,
            measuringUnits = unitsResult.Data
        };
    }
}
