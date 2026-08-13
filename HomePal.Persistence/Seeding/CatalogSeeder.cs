using System.Text.Json;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomePal.Persistence.Seeding;

public static class CatalogSeeder
{
    public static async Task SeedCatalogDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var environment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CatalogSeeder");

        logger.LogInformation("Checking and executing database seed data...");

        // 1. Locate and parse seed.json
        var seedJsonPath = FindSeedJsonPath();
        if (seedJsonPath == null || !File.Exists(seedJsonPath))
        {
            logger.LogWarning("seed.json not found. Skipping JSON seeding.");
            return;
        }

        var jsonContent = await File.ReadAllTextAsync(seedJsonPath);
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var seedData = JsonSerializer.Deserialize<SeedDataDto>(jsonContent, jsonOptions);

        if (seedData == null)
        {
            logger.LogWarning("Failed to deserialize seed.json.");
            return;
        }

        // 2. Seed Measuring Units
        if (seedData.MeasuringUnits != null && seedData.MeasuringUnits.Count > 0)
        {
            var existingUnits = await dbContext.MeasuringUnits.ToListAsync();
            var unitsToAdd = new List<MeasuringUnit>();

            foreach (var unitDto in seedData.MeasuringUnits)
            {
                bool exists = existingUnits.Any(u =>
                    u.Name.Any(n => n.Value.Equals(unitDto.NameEn, StringComparison.OrdinalIgnoreCase) ||
                                    n.Value.Equals(unitDto.NameAr, StringComparison.OrdinalIgnoreCase)) ||
                    u.Symbol.Any(s => s.Value.Equals(unitDto.SymbolEn, StringComparison.OrdinalIgnoreCase) ||
                                      s.Value.Equals(unitDto.SymbolAr, StringComparison.OrdinalIgnoreCase)));

                if (!exists)
                {
                    var newUnit = new MeasuringUnit
                    {
                        Id = Guid.NewGuid(),
                        Name = [new LocalizedItem("en", unitDto.NameEn), new LocalizedItem("ar", unitDto.NameAr)],
                        Symbol = [new LocalizedItem("en", unitDto.SymbolEn), new LocalizedItem("ar", unitDto.SymbolAr)],
                        CreatedAt = DateTime.UtcNow
                    };
                    unitsToAdd.Add(newUnit);
                    existingUnits.Add(newUnit);
                }
            }

            if (unitsToAdd.Count > 0)
            {
                await dbContext.MeasuringUnits.AddRangeAsync(unitsToAdd);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} new measuring units.", unitsToAdd.Count);
            }
        }

        // 3. Seed Product Categories
        if (seedData.ProductCategories != null && seedData.ProductCategories.Count > 0)
        {
            var existingCategories = await dbContext.ProductCategories.ToListAsync();
            var categoriesToAdd = new List<ProductCategory>();

            foreach (var catDto in seedData.ProductCategories)
            {
                bool exists = existingCategories.Any(c =>
                    c.Name.Any(n => n.Value.Equals(catDto.NameEn, StringComparison.OrdinalIgnoreCase) ||
                                    n.Value.Equals(catDto.NameAr, StringComparison.OrdinalIgnoreCase)));

                if (!exists)
                {
                    var newCat = new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = [new LocalizedItem("en", catDto.NameEn), new LocalizedItem("ar", catDto.NameAr)],
                        Description = [new LocalizedItem("en", catDto.DescriptionEn), new LocalizedItem("ar", catDto.DescriptionAr)],
                        ImagePath = catDto.ImagePath,
                        CreatedAt = DateTime.UtcNow
                    };
                    categoriesToAdd.Add(newCat);
                    existingCategories.Add(newCat);
                }
            }

            if (categoriesToAdd.Count > 0)
            {
                await dbContext.ProductCategories.AddRangeAsync(categoriesToAdd);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} new product categories.", categoriesToAdd.Count);
            }
        }

        // 4. Seed Preference Categories
        var prefCatMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        if (seedData.PreferenceCategories != null && seedData.PreferenceCategories.Count > 0)
        {
            var existingPrefCats = await dbContext.PreferenceCategories.ToListAsync();
            var prefCatsToAdd = new List<PreferenceCategory>();

            foreach (var prefCatDto in seedData.PreferenceCategories)
            {
                var match = existingPrefCats.FirstOrDefault(pc =>
                    pc.Name.Any(n => n.Value.Equals(prefCatDto.NameEn, StringComparison.OrdinalIgnoreCase) ||
                                     n.Value.Equals(prefCatDto.NameAr, StringComparison.OrdinalIgnoreCase)));

                if (match == null)
                {
                    var newPrefCat = new PreferenceCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = [new LocalizedItem("en", prefCatDto.NameEn), new LocalizedItem("ar", prefCatDto.NameAr)],
                        Description = [new LocalizedItem("en", prefCatDto.DescriptionEn), new LocalizedItem("ar", prefCatDto.DescriptionAr)],
                        CreatedAt = DateTime.UtcNow
                    };
                    prefCatsToAdd.Add(newPrefCat);
                    existingPrefCats.Add(newPrefCat);
                    prefCatMap[prefCatDto.NameEn] = newPrefCat.Id;
                    prefCatMap[prefCatDto.NameAr] = newPrefCat.Id;
                }
                else
                {
                    prefCatMap[prefCatDto.NameEn] = match.Id;
                    prefCatMap[prefCatDto.NameAr] = match.Id;
                }
            }

            if (prefCatsToAdd.Count > 0)
            {
                await dbContext.PreferenceCategories.AddRangeAsync(prefCatsToAdd);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} new preference categories.", prefCatsToAdd.Count);
            }
        }

        // 5. Seed Preferences
        if (seedData.Preferences != null && seedData.Preferences.Count > 0)
        {
            var allPrefCats = await dbContext.PreferenceCategories.ToListAsync();
            var existingPreferences = await dbContext.Preferences.ToListAsync();
            var preferencesToAdd = new List<Preference>();

            foreach (var prefDto in seedData.Preferences)
            {
                // Resolve parent category
                Guid catId = Guid.Empty;
                if (prefCatMap.TryGetValue(prefDto.PreferenceCategory, out var mappedId))
                {
                    catId = mappedId;
                }
                else
                {
                    var matchedCat = allPrefCats.FirstOrDefault(pc =>
                        pc.Name.Any(n => n.Value.Equals(prefDto.PreferenceCategory, StringComparison.OrdinalIgnoreCase)));
                    if (matchedCat != null)
                    {
                        catId = matchedCat.Id;
                    }
                }

                if (catId == Guid.Empty)
                {
                    logger.LogWarning("Preference '{NameEn}' references unknown category '{Cat}'. Skipping.", prefDto.NameEn, prefDto.PreferenceCategory);
                    continue;
                }

                bool exists = existingPreferences.Any(p =>
                    p.CategoryId == catId &&
                    p.Name.Any(n => n.Value.Equals(prefDto.NameEn, StringComparison.OrdinalIgnoreCase) ||
                                    n.Value.Equals(prefDto.NameAr, StringComparison.OrdinalIgnoreCase)));

                if (!exists)
                {
                    var newPref = new Preference
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = catId,
                        Name = [new LocalizedItem("en", prefDto.NameEn), new LocalizedItem("ar", prefDto.NameAr)],
                        Description = [new LocalizedItem("en", prefDto.DescriptionEn), new LocalizedItem("ar", prefDto.DescriptionAr)],
                        CreatedAt = DateTime.UtcNow
                    };
                    preferencesToAdd.Add(newPref);
                    existingPreferences.Add(newPref);
                }
            }

            if (preferencesToAdd.Count > 0)
            {
                await dbContext.Preferences.AddRangeAsync(preferencesToAdd);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} new preferences.", preferencesToAdd.Count);
            }
        }

        // 6. Seed Supermarkets & Sample Offers if not present
        if (!await dbContext.Supermarkets.AnyAsync())
        {
            await SeedSupermarketsAndOffersAsync(dbContext, environment, logger);
        }

        logger.LogInformation("Database seed check and execution completed successfully.");
    }

    private static async Task SeedSupermarketsAndOffersAsync(
        ApplicationDbContext dbContext,
        IWebHostEnvironment? environment,
        ILogger logger)
    {
        logger.LogInformation("Seeding Supermarkets and Sample Offers...");

        var marketCarrefour = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Carrefour"), new LocalizedItem("ar", "كارفور")],
            Address = "Cairo Festival City, New Cairo",
            WebsiteUrl = "https://www.carrefouregypt.com",
            CreatedAt = DateTime.UtcNow
        };
        var marketLulu = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "LuLu Hypermarket"), new LocalizedItem("ar", "اللولو هايبر ماركت")],
            Address = "Twin Plaza, First Settlement",
            WebsiteUrl = "https://www.luluhypermarket.com",
            CreatedAt = DateTime.UtcNow
        };
        var marketSpinneys = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Spinneys"), new LocalizedItem("ar", "سبينيس")],
            Address = "City Centre Almaza, Heliopolis",
            WebsiteUrl = "https://spinneys-egypt.com",
            CreatedAt = DateTime.UtcNow
        };
        var marketMetro = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Metro Market"), new LocalizedItem("ar", "مترو ماركت")],
            Address = "90th Street, Fifth Settlement",
            WebsiteUrl = "https://metro-markets.com",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Supermarkets.AddRange(marketCarrefour, marketLulu, marketSpinneys, marketMetro);
        await dbContext.SaveChangesAsync();

        var unitL = await dbContext.MeasuringUnits.FirstOrDefaultAsync(u => u.Symbol.Any(s => s.Value == "L" || s.Value == "l"));
        var unitKg = await dbContext.MeasuringUnits.FirstOrDefaultAsync(u => u.Symbol.Any(s => s.Value == "kg" || s.Value == "كجم"));
        var unitPack = await dbContext.MeasuringUnits.FirstOrDefaultAsync(u => u.Symbol.Any(s => s.Value == "pack" || s.Value == "عبوة"));

        var catDairy = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Name.Any(n => n.Value.Contains("Dairy") || n.Value.Contains("الألبان")));
        var catProduce = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Name.Any(n => n.Value.Contains("Fruit") || n.Value.Contains("Produce") || n.Value.Contains("الفواكه")));
        var catMeat = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Name.Any(n => n.Value.Contains("Meat") || n.Value.Contains("اللحوم") || n.Value.Contains("Poultry")));

        if (unitL != null && catDairy != null)
        {
            var now = DateTime.UtcNow;
            var offers = new List<Offer>
            {
                new Offer
                {
                    Id = Guid.NewGuid(),
                    Name = [new LocalizedItem("en", "Full Cream Milk 1L Offer"), new LocalizedItem("ar", "عرض حليب كامل الدسم 1 لتر")],
                    Description = [new LocalizedItem("en", "Special discount on 1L fresh milk"), new LocalizedItem("ar", "خصم خاص على الحليب الطازج 1 لتر")],
                    Quantity = 1,
                    UnitId = unitL.Id,
                    OriginalPrice = 45.00m,
                    DiscountedPrice = 36.50m,
                    ValidFrom = now.AddDays(-2),
                    ValidTo = now.AddDays(10),
                    CategoryId = catDairy.Id,
                    SupermarketId = marketCarrefour.Id,
                    CreatedAt = now
                }
            };

            if (unitPack != null)
            {
                offers.Add(new Offer
                {
                    Id = Guid.NewGuid(),
                    Name = [new LocalizedItem("en", "Farm Eggs 30s Big Saver"), new LocalizedItem("ar", "عرض البيض 30 بيضة توفير كبير")],
                    Description = [new LocalizedItem("en", "Pack of 30 eggs at lowest price"), new LocalizedItem("ar", "طبق بيض 30 بيضة بأقل سعر")],
                    Quantity = 1,
                    UnitId = unitPack.Id,
                    OriginalPrice = 160.00m,
                    DiscountedPrice = 139.99m,
                    ValidFrom = now.AddDays(-1),
                    ValidTo = now.AddDays(7),
                    CategoryId = catDairy.Id,
                    SupermarketId = marketLulu.Id,
                    CreatedAt = now
                });
            }

            if (unitKg != null && catProduce != null)
            {
                offers.Add(new Offer
                {
                    Id = Guid.NewGuid(),
                    Name = [new LocalizedItem("en", "Fresh Bananas 1kg Deal"), new LocalizedItem("ar", "صفقة الموز الطازج 1 كيلو")],
                    Description = [new LocalizedItem("en", "Freshly picked yellow bananas"), new LocalizedItem("ar", "موز أصفر طازج مقطوف حديثاً")],
                    Quantity = 1,
                    UnitId = unitKg.Id,
                    OriginalPrice = 35.00m,
                    DiscountedPrice = 24.99m,
                    ValidFrom = now.AddDays(-3),
                    ValidTo = now.AddDays(5),
                    CategoryId = catProduce.Id,
                    SupermarketId = marketSpinneys.Id,
                    CreatedAt = now
                });
            }

            if (unitKg != null && catMeat != null)
            {
                offers.Add(new Offer
                {
                    Id = Guid.NewGuid(),
                    Name = [new LocalizedItem("en", "Chicken Breast Fillet 1kg Super Sale"), new LocalizedItem("ar", "تخفيضات سوبر على صدور الدجاج 1 كيلو")],
                    Description = [new LocalizedItem("en", "Premium fresh boneless chicken breasts"), new LocalizedItem("ar", "صدور دجاج مخلية ممتازة")],
                    Quantity = 1,
                    UnitId = unitKg.Id,
                    OriginalPrice = 240.00m,
                    DiscountedPrice = 195.00m,
                    ValidFrom = now.AddDays(-1),
                    ValidTo = now.AddDays(6),
                    CategoryId = catMeat.Id,
                    SupermarketId = marketCarrefour.Id,
                    CreatedAt = now
                });
            }

            await dbContext.Offers.AddRangeAsync(offers);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded Supermarkets and {Count} sample offers.", offers.Count);
        }
    }

    private static string? FindSeedJsonPath()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Seeding", "seed.json"),
            Path.Combine(AppContext.BaseDirectory, "seed.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "HomePal.Persistence", "Seeding", "seed.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "HomePal.Persistence", "Seeding", "seed.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "seed.json")
        };

        return candidates.FirstOrDefault(File.Exists);
    }

    private class SeedDataDto
    {
        public List<MeasuringUnitSeedDto>? MeasuringUnits { get; set; }
        public List<ProductCategorySeedDto>? ProductCategories { get; set; }
        public List<PreferenceCategorySeedDto>? PreferenceCategories { get; set; }
        public List<PreferenceSeedDto>? Preferences { get; set; }
    }

    private class MeasuringUnitSeedDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string SymbolAr { get; set; } = string.Empty;
        public string SymbolEn { get; set; } = string.Empty;
    }

    private class ProductCategorySeedDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }

    private class PreferenceCategorySeedDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
    }

    private class PreferenceSeedDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string PreferenceCategory { get; set; } = string.Empty;
    }
}
