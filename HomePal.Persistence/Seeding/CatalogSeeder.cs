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
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CatalogSeeder");

        if (await dbContext.ProductCategories.AnyAsync())
        {
            logger.LogInformation("Catalog data already exists. Skipping catalog seeding.");
            return;
        }

        logger.LogInformation("Seeding Catalog dummy data...");

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // 1. Measuring Units
        var unitKg = new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Symbol = [new LocalizedItem("en", "kg"), new LocalizedItem("ar", "كجم")],
            Name = [new LocalizedItem("en", "Kilogram"), new LocalizedItem("ar", "كيلوجرام")],
            CreatedAt = DateTime.UtcNow
        };
        var unitG = new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Symbol = [new LocalizedItem("en", "g"), new LocalizedItem("ar", "جم")],
            Name = [new LocalizedItem("en", "Gram"), new LocalizedItem("ar", "جرام")],
            CreatedAt = DateTime.UtcNow
        };
        var unitL = new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Symbol = [new LocalizedItem("en", "L"), new LocalizedItem("ar", "لتر")],
            Name = [new LocalizedItem("en", "Liter"), new LocalizedItem("ar", "لتر")],
            CreatedAt = DateTime.UtcNow
        };
        var unitPcs = new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Symbol = [new LocalizedItem("en", "pcs"), new LocalizedItem("ar", "قطعة")],
            Name = [new LocalizedItem("en", "Piece"), new LocalizedItem("ar", "قطعة")],
            CreatedAt = DateTime.UtcNow
        };
        var unitPack = new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Symbol = [new LocalizedItem("en", "pack"), new LocalizedItem("ar", "عبوة")],
            Name = [new LocalizedItem("en", "Pack"), new LocalizedItem("ar", "عبوة")],
            CreatedAt = DateTime.UtcNow
        };

        dbContext.MeasuringUnits.AddRange(unitKg, unitG, unitL, unitPcs, unitPack);
        await dbContext.SaveChangesAsync();

        // Helper to get dummy image
        async Task<string> DownloadDummyImageAsync(string folder, string label)
        {
            return await SaveSampleImageAsync(environment, httpClient, folder, label);
        }

        // 2. Product Categories
        var catDairy = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Dairy & Eggs"), new LocalizedItem("ar", "ألبان وبيض")],
            Description = [new LocalizedItem("en", "Fresh milk, cheese, eggs and dairy products"), new LocalizedItem("ar", "حليب طازج، أجبان، بيض ومنتجات الألبان")],
            ImagePath = await DownloadDummyImageAsync("categories", "Dairy"),
            CreatedAt = DateTime.UtcNow
        };
        var catProduce = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Fruits & Vegetables"), new LocalizedItem("ar", "فواكه وخضروات")],
            Description = [new LocalizedItem("en", "Fresh organic fruits and vegetables"), new LocalizedItem("ar", "فواكه وخضروات طازجة وعضوية")],
            ImagePath = await DownloadDummyImageAsync("categories", "Produce"),
            CreatedAt = DateTime.UtcNow
        };
        var catBeverages = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Beverages"), new LocalizedItem("ar", "مشروبات")],
            Description = [new LocalizedItem("en", "Juices, soft drinks, water and hot beverages"), new LocalizedItem("ar", "عصائر، مشروبات غازية، مياه ومشروبات ساخنة")],
            ImagePath = await DownloadDummyImageAsync("categories", "Beverages"),
            CreatedAt = DateTime.UtcNow
        };
        var catBakery = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Bakery"), new LocalizedItem("ar", "مخبوزات")],
            Description = [new LocalizedItem("en", "Fresh bread, cakes, pastries and baked goods"), new LocalizedItem("ar", "خبز طازج، كيك، فطائر ومخبوزات")],
            ImagePath = await DownloadDummyImageAsync("categories", "Bakery"),
            CreatedAt = DateTime.UtcNow
        };
        var catMeat = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Meat & Poultry"), new LocalizedItem("ar", "لحوم ودواجن")],
            Description = [new LocalizedItem("en", "Fresh beef, chicken, lamb and poultry"), new LocalizedItem("ar", "لحم بقر طازج، دجاج، لحم ضأن ودواجن")],
            ImagePath = await DownloadDummyImageAsync("categories", "Meat"),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.ProductCategories.AddRange(catDairy, catProduce, catBeverages, catBakery, catMeat);
        await dbContext.SaveChangesAsync();

        // 3. Supermarkets
        var marketCarrefour = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Carrefour"), new LocalizedItem("ar", "كارفور")],
            Address = "Cairo Festival City, New Cairo",
            WebsiteUrl = "https://www.carrefouregypt.com",
            LogoPath = await DownloadDummyImageAsync("supermarkets", "Carrefour"),
            CreatedAt = DateTime.UtcNow
        };
        var marketLulu = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "LuLu Hypermarket"), new LocalizedItem("ar", "اللولو هايبر ماركت")],
            Address = "Twin Plaza, First Settlement",
            WebsiteUrl = "https://www.luluhypermarket.com",
            LogoPath = await DownloadDummyImageAsync("supermarkets", "Lulu"),
            CreatedAt = DateTime.UtcNow
        };
        var marketSpinneys = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Spinneys"), new LocalizedItem("ar", "سبينيس")],
            Address = "City Centre Almaza, Heliopolis",
            WebsiteUrl = "https://spinneys-egypt.com",
            LogoPath = await DownloadDummyImageAsync("supermarkets", "Spinneys"),
            CreatedAt = DateTime.UtcNow
        };
        var marketMetro = new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Metro Market"), new LocalizedItem("ar", "مترو ماركت")],
            Address = "90th Street, Fifth Settlement",
            WebsiteUrl = "https://metro-markets.com",
            LogoPath = await DownloadDummyImageAsync("supermarkets", "Metro"),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Supermarkets.AddRange(marketCarrefour, marketLulu, marketSpinneys, marketMetro);
        await dbContext.SaveChangesAsync();

        // 4. Canonical Products
        var prodMilk = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Fresh Full Cream Milk 1L"), new LocalizedItem("ar", "حليب كامل الدسم طازج 1 لتر")],
            Description = [new LocalizedItem("en", "Pasteurized full cream fresh milk"), new LocalizedItem("ar", "حليب طازج مبستر كامل الدسم")],
            CategoryId = catDairy.Id,
            ImagePath = await DownloadDummyImageAsync("products", "Milk"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };
        var prodEggs = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Farm Fresh White Eggs 30 Pack"), new LocalizedItem("ar", "بيض أبيض طازج كرتونة 30 بيضة")],
            Description = [new LocalizedItem("en", "Grade A fresh white eggs 30 pieces"), new LocalizedItem("ar", "بيض أبيض طازج فئة أ 30 قطعة")],
            CategoryId = catDairy.Id,
            ImagePath = await DownloadDummyImageAsync("products", "Eggs"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };
        var prodBananas = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Fresh Yellow Bananas 1kg"), new LocalizedItem("ar", "موز أصفر طازج 1 كيلو")],
            Description = [new LocalizedItem("en", "Sweet premium yellow bananas"), new LocalizedItem("ar", "موز أصفر ممتاز عالي الجودة")],
            CategoryId = catProduce.Id,
            ImagePath = await DownloadDummyImageAsync("products", "Bananas"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };
        var prodOrangeJuice = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Natural Orange Juice 1L"), new LocalizedItem("ar", "عصير برتقال طبيعي 1 لتر")],
            Description = [new LocalizedItem("en", "100% natural pure orange juice"), new LocalizedItem("ar", "عصير برتقال طبيعي نقي 100%")],
            CategoryId = catBeverages.Id,
            ImagePath = await DownloadDummyImageAsync("products", "OrangeJuice"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };
        var prodBread = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "White Toast Bread 500g"), new LocalizedItem("ar", "خبز توست أبيض 500 جرام")],
            Description = [new LocalizedItem("en", "Soft sliced white toast bread"), new LocalizedItem("ar", "خبز توست أبيض شرائح طري")],
            CategoryId = catBakery.Id,
            ImagePath = await DownloadDummyImageAsync("products", "ToastBread"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };
        var prodChicken = new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = [new LocalizedItem("en", "Fresh Chicken Breast Fillet 1kg"), new LocalizedItem("ar", "صدور دجاج مخلية طازجة 1 كيلو")],
            Description = [new LocalizedItem("en", "Boneless skinless fresh chicken breast"), new LocalizedItem("ar", "صدور دجاج طازجة بدون عظم أو جلد")],
            CategoryId = catMeat.Id,
            ImagePath = await DownloadDummyImageAsync("products", "ChickenBreast"),
            Embedding = null,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.CanonicalProducts.AddRange(prodMilk, prodEggs, prodBananas, prodOrangeJuice, prodBread, prodChicken);
        await dbContext.SaveChangesAsync();

        // 5. Offers
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
                CanonicalProductId = prodMilk.Id,
                ImagePath = await DownloadDummyImageAsync("offers", "OfferMilkCarrefour"),
                Embedding = null,
                CreatedAt = now
            },
            new Offer
            {
                Id = Guid.NewGuid(),
                Name = [new LocalizedItem("en", "Farm Eggs 30s Big Saver"), new LocalizedItem("ar", "عرض البيض 30 بيضة توفير كبيير")],
                Description = [new LocalizedItem("en", "Pack of 30 eggs at lowest price"), new LocalizedItem("ar", "طبق بيض 30 بيضة بأقل سعر")],
                Quantity = 1,
                UnitId = unitPack.Id,
                OriginalPrice = 160.00m,
                DiscountedPrice = 139.99m,
                ValidFrom = now.AddDays(-1),
                ValidTo = now.AddDays(7),
                CategoryId = catDairy.Id,
                SupermarketId = marketLulu.Id,
                CanonicalProductId = prodEggs.Id,
                ImagePath = await DownloadDummyImageAsync("offers", "OfferEggsLulu"),
                Embedding = null,
                CreatedAt = now
            },
            new Offer
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
                CanonicalProductId = prodBananas.Id,
                ImagePath = await DownloadDummyImageAsync("offers", "OfferBananasSpinneys"),
                Embedding = null,
                CreatedAt = now
            },
            new Offer
            {
                Id = Guid.NewGuid(),
                Name = [new LocalizedItem("en", "Pure Orange Juice 1L Promo"), new LocalizedItem("ar", "عرض عصير البرتقال الطبيعي 1 لتر")],
                Description = [new LocalizedItem("en", "Buy 1L natural juice at discount"), new LocalizedItem("ar", "اشتر عصير طبيعي 1 لتر بخصم")],
                Quantity = 1,
                UnitId = unitL.Id,
                OriginalPrice = 50.00m,
                DiscountedPrice = 39.50m,
                ValidFrom = now.AddDays(-5),
                ValidTo = now.AddDays(12),
                CategoryId = catBeverages.Id,
                SupermarketId = marketMetro.Id,
                CanonicalProductId = prodOrangeJuice.Id,
                ImagePath = await DownloadDummyImageAsync("offers", "OfferJuiceMetro"),
                Embedding = null,
                CreatedAt = now
            },
            new Offer
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
                CanonicalProductId = prodChicken.Id,
                ImagePath = await DownloadDummyImageAsync("offers", "OfferChickenCarrefour"),
                Embedding = null,
                CreatedAt = now
            }
        };

        dbContext.Offers.AddRange(offers);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Catalog dummy data seeded successfully.");
    }

    private static async Task<string> SaveSampleImageAsync(
        IWebHostEnvironment environment,
        HttpClient httpClient,
        string folder,
        string text)
    {
        var webRootPath = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        }

        var folderPath = Path.Combine(webRootPath, "uploads", folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.NewGuid():N}.jpg";
        var filePath = Path.Combine(folderPath, fileName);

        byte[]? imageBytes = null;

        // Try downloading random image from reliable placeholder service
        try
        {
            var placeholderUrl = $"https://dummyimage.com/400x300/3498db/ffffff.jpg&text={Uri.EscapeDataString(text)}";
            imageBytes = await httpClient.GetByteArrayAsync(placeholderUrl);
        }
        catch
        {
            // Ignore network errors and fall back to generated byte array
        }

        if (imageBytes == null || imageBytes.Length == 0)
        {
            imageBytes = GenerateFallbackJpegBytes();
        }

        await File.WriteAllBytesAsync(filePath, imageBytes);
        return $"/uploads/{folder}/{fileName}";
    }

    private static byte[] GenerateFallbackJpegBytes()
    {
        // Minimal valid 1x1 JPEG image byte array
        return [
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48,
            0x00, 0x48, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08,
            0x07, 0x07, 0x07, 0x09, 0x09, 0x08, 0x0A, 0x0C, 0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12,
            0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20, 0x24, 0x2E, 0x27, 0x20,
            0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29, 0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27,
            0x39, 0x3D, 0x38, 0x32, 0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01,
            0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x1F, 0x00, 0x00, 0x01, 0x05, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04,
            0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0xFF, 0xC4, 0x00, 0xB5, 0x10, 0x00, 0x02, 0x01, 0x03,
            0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x01, 0x7D, 0x01, 0x02, 0x03, 0x00, 0x04,
            0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81,
            0x91, 0xA1, 0x08, 0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72, 0x82,
            0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56,
            0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75, 0x76,
            0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95,
            0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3,
            0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA,
            0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7,
            0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFF, 0xDA, 0x00,
            0x08, 0x01, 0x01, 0x00, 0x00, 0x3F, 0x00, 0xFB, 0xD0, 0x7F, 0xFF, 0xD9
        ];
    }
}
