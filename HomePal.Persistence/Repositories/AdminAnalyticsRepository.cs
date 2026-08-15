using System.Globalization;
using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Domain.Common;
using HomePal.Domain.Constants;
using HomePal.Domain.Enums;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class AdminAnalyticsRepository : IAdminAnalyticsRepository
{
    private readonly ApplicationDbContext _context;

    public AdminAnalyticsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync(CancellationToken cancellationToken = default)
    {
        // 1. Top Supermarket Chains (based on purchased items from offers)
        var purchasedItems = await _context.ShoppingListItems
            .AsNoTracking()
            .Include(sli => sli.Offer)
                .ThenInclude(o => o!.Supermarket)
            .Where(sli => sli.IsPurchased && !sli.IsDeleted && sli.Offer != null && sli.Offer.Supermarket != null)
            .ToListAsync(cancellationToken);

        var topChains = purchasedItems
            .Where(sli => sli.Offer?.Supermarket != null)
            .GroupBy(sli => sli.Offer!.SupermarketId)
            .Select(g => new TopSupermarketChainDto
            {
                Name = g.First().Offer!.Supermarket!.Name.Get(),
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .Take(5)
            .ToList();

        // 2. User / Household Regional Distribution
        var householdsWithLocation = await _context.Households
            .AsNoTracking()
            .Include(h => h.City)
            .Include(h => h.Governorate)
            .Where(h => !h.IsDeleted)
            .ToListAsync(cancellationToken);

        var userDistribution = householdsWithLocation
            .Where(h => h.City != null || h.Governorate != null)
            .GroupBy(h => h.City != null ? $"C_{h.CityId}" : $"G_{h.GovernorateId}")
            .Select(g =>
            {
                var first = g.First();
                var regionName = first.City != null ? first.City.Name.Get() : first.Governorate!.Name.Get();
                var lat = first.City?.Latitude ?? first.Governorate?.Latitude ?? 0;
                var lng = first.City?.Longitude ?? first.Governorate?.Longitude ?? 0;

                return new UserDistributionDto
                {
                    Region = regionName,
                    Households = g.Count(),
                    Lat = lat,
                    Lng = lng
                };
            })
            .OrderByDescending(x => x.Households)
            .Take(10)
            .ToList();

        // 3. Top Categories
        var shoppingCategoryItems = await _context.ShoppingListItems
            .AsNoTracking()
            .Include(sli => sli.Category)
            .Where(sli => !sli.IsDeleted && sli.Category != null)
            .ToListAsync(cancellationToken);

        var totalItems = shoppingCategoryItems.Count;
        var topCategories = new List<CategoryShareDto>();

        if (totalItems > 0)
        {
            topCategories = shoppingCategoryItems
                .GroupBy(sli => sli.CategoryId)
                .Select(g => new CategoryShareDto
                {
                    Name = g.First().Category!.Name.Get(),
                    Percentage = Math.Round((double)g.Count() / totalItems * 100, 1)
                })
                .OrderByDescending(c => c.Percentage)
                .Take(5)
                .ToList();
        }
        else
        {
            var pantryItems = await _context.PantryItems
                .AsNoTracking()
                .Include(pi => pi.Category)
                .Where(pi => !pi.IsDeleted && pi.Category != null)
                .ToListAsync(cancellationToken);

            var pantryTotal = pantryItems.Count;
            if (pantryTotal > 0)
            {
                topCategories = pantryItems
                    .GroupBy(pi => pi.CategoryId)
                    .Select(g => new CategoryShareDto
                    {
                        Name = g.First().Category!.Name.Get(),
                        Percentage = Math.Round((double)g.Count() / pantryTotal * 100, 1)
                    })
                    .OrderByDescending(c => c.Percentage)
                    .Take(5)
                    .ToList();
            }
        }

        return new AnalyticsOverviewDto
        {
            TopSupermarketChains = topChains,
            UserDistribution = userDistribution,
            TopCategories = topCategories
        };
    }

    public async Task<GeographicDemographicsDto> GetGeographicDemographicsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfCurrentMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfLastMonth = startOfCurrentMonth.AddMonths(-1);

        var allHouseholds = await _context.Households
            .AsNoTracking()
            .Include(h => h.City)
            .Include(h => h.Governorate)
            .Include(h => h.Members)
            .Include(h => h.Budgets)
            .Where(h => !h.IsDeleted)
            .ToListAsync(cancellationToken);

        var allUsers = await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);

        // 1. Districts Breakdown
        var cityGroups = allHouseholds
            .Where(h => h.City != null)
            .GroupBy(h => h.CityId!.Value)
            .ToList();

        var maxHhInCity = cityGroups.Count > 0 ? cityGroups.Max(g => g.Count()) : 1;
        var districts = new List<DistrictDemographicDto>();

        foreach (var group in cityGroups.Take(15))
        {
            var city = group.First().City!;
            var hhCount = group.Count();
            var popCount = group.Sum(h => h.Members.Count(m => !m.IsDeleted));

            var currentMonthNewHh = group.Count(h => h.CreatedAt >= startOfCurrentMonth);
            var lastMonthNewHh = group.Count(h => h.CreatedAt >= startOfLastMonth && h.CreatedAt < startOfCurrentMonth);
            var growthPct = lastMonthNewHh > 0
                ? ((double)(currentMonthNewHh - lastMonthNewHh) / lastMonthNewHh) * 100
                : (currentMonthNewHh > 0 ? 100.0 : 0.0);

            var avgCityBudget = group
                .SelectMany(h => h.Budgets)
                .Where(b => !b.IsDeleted)
                .Select(b => b.Amount)
                .DefaultIfEmpty(0m)
                .Average();

            var intensity = Math.Round(Math.Clamp((double)hhCount / maxHhInCity, 0.0, 1.0), 2);
            var hhDensity = hhCount >= 10 ? "High" : (hhCount >= 4 ? "Medium" : "Low");

            districts.Add(new DistrictDemographicDto
            {
                Id = city.Id.ToString(),
                Name = city.Name.Get(),
                Growth = $"{(growthPct >= 0 ? "+" : "")}{growthPct:0.#}%",
                HhDensity = hhDensity,
                AvgIncome = $"{avgCityBudget:N0} AED",
                Pop = popCount.ToString("N0", CultureInfo.InvariantCulture),
                Intensity = intensity,
                Lat = city.Latitude,
                Lng = city.Longitude,
                Radius = 1500
            });
        }

        // 2. Budget Metrics
        var currentBudgets = await _context.HouseholdMonthlyBudgets
            .AsNoTracking()
            .Where(b => !b.IsDeleted && b.BudgetDate >= startOfCurrentMonth)
            .Select(b => b.Amount)
            .ToListAsync(cancellationToken);

        var lastBudgets = await _context.HouseholdMonthlyBudgets
            .AsNoTracking()
            .Where(b => !b.IsDeleted && b.BudgetDate >= startOfLastMonth && b.BudgetDate < startOfCurrentMonth)
            .Select(b => b.Amount)
            .ToListAsync(cancellationToken);

        var avgCurrentBudget = currentBudgets.Count > 0 ? currentBudgets.Average() : 0m;
        var avgLastBudget = lastBudgets.Count > 0 ? lastBudgets.Average() : 0m;
        var budgetChangePct = avgLastBudget > 0 ? ((avgCurrentBudget - avgLastBudget) / avgLastBudget) * 100 : 0;

        var budgetMetric = new BudgetMetricDto
        {
            Value = $"{avgCurrentBudget:N0} AED",
            Change = $"{(budgetChangePct >= 0 ? "+" : "")}{budgetChangePct:0.#}%",
            Region = "Citywide"
        };

        var allBudgets = await _context.HouseholdMonthlyBudgets
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .Select(b => b.Amount)
            .ToListAsync(cancellationToken);

        var avgBudgetPerHh = allBudgets.Count > 0 ? allBudgets.Average() : 0m;

        // 3. Top Categories
        var categoryPurchases = await _context.ShoppingListItems
            .AsNoTracking()
            .Include(sli => sli.Category)
            .Where(sli => !sli.IsDeleted && sli.Category != null)
            .ToListAsync(cancellationToken);

        var totalPurchases = categoryPurchases.Count;
        var topCategories = categoryPurchases
            .GroupBy(sli => sli.CategoryId)
            .Select(g => new CategoryShareDto
            {
                Name = g.First().Category!.Name.Get(),
                Percentage = totalPurchases > 0 ? Math.Round((double)g.Count() / totalPurchases * 100, 1) : 0
            })
            .OrderByDescending(c => c.Percentage)
            .Take(5)
            .ToList();

        // 4. Household Size Distribution
        var totalHhCount = allHouseholds.Count;
        var size1To2 = 0;
        var size3To4 = 0;
        var size5Plus = 0;

        foreach (var hh in allHouseholds)
        {
            var memberCount = hh.Members.Count(m => !m.IsDeleted);
            if (memberCount <= 2) size1To2++;
            else if (memberCount <= 4) size3To4++;
            else size5Plus++;
        }

        var householdSize = new List<HouseholdSizeDistributionDto>
        {
            new() { Size = "1-2 Members", Value = totalHhCount > 0 ? Math.Round((double)size1To2 / totalHhCount * 100, 1) : 0 },
            new() { Size = "3-4 Members", Value = totalHhCount > 0 ? Math.Round((double)size3To4 / totalHhCount * 100, 1) : 0 },
            new() { Size = "5+ Members", Value = totalHhCount > 0 ? Math.Round((double)size5Plus / totalHhCount * 100, 1) : 0 }
        };

        // 5. Gender Split
        var femaleCount = allUsers.Count(u => u.Gender == Gender.Female);
        var maleCount = allUsers.Count(u => u.Gender == Gender.Male);
        var totalGender = femaleCount + maleCount;

        var genderSplit = new List<GenderSplitDto>
        {
            new() { Gender = "Female", Percentage = totalGender > 0 ? Math.Round((double)femaleCount / totalGender * 100, 1) : 0 },
            new() { Gender = "Male", Percentage = totalGender > 0 ? Math.Round((double)maleCount / totalGender * 100, 1) : 0 }
        };

        // 6. Age Split
        var age18To24 = 0;
        var age25To34 = 0;
        var age35To44 = 0;
        var age45Plus = 0;

        foreach (var user in allUsers)
        {
            var age = CalculateAge(user.BirthDate);
            if (age >= 18 && age <= 24) age18To24++;
            else if (age >= 25 && age <= 34) age25To34++;
            else if (age >= 35 && age <= 44) age35To44++;
            else if (age >= 45) age45Plus++;
        }

        var totalAge = age18To24 + age25To34 + age35To44 + age45Plus;
        var ageSplit = new List<AgeSplitDto>
        {
            new() { Range = "18-24", Percentage = totalAge > 0 ? Math.Round((double)age18To24 / totalAge * 100, 1) : 0 },
            new() { Range = "25-34", Percentage = totalAge > 0 ? Math.Round((double)age25To34 / totalAge * 100, 1) : 0 },
            new() { Range = "35-44", Percentage = totalAge > 0 ? Math.Round((double)age35To44 / totalAge * 100, 1) : 0 },
            new() { Range = "45+", Percentage = totalAge > 0 ? Math.Round((double)age45Plus / totalAge * 100, 1) : 0 }
        };

        return new GeographicDemographicsDto
        {
            Districts = districts,
            Budget = budgetMetric,
            AvgBudgetPerHousehold = $"{avgBudgetPerHh:N0} AED",
            TopCategories = topCategories,
            HouseholdSize = householdSize,
            GenderSplit = genderSplit,
            AgeSplit = ageSplit
        };
    }

    public async Task<HouseholdsSummaryDto> GetHouseholdsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfCurrentMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfLastMonth = startOfCurrentMonth.AddMonths(-1);

        var households = await _context.Households
            .AsNoTracking()
            .Include(h => h.Members)
            .Include(h => h.City)
            .Include(h => h.Governorate)
            .Where(h => !h.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalHouseholds = households.Count;
        var activeHouseholds = households.Count(h => h.Members.Any(m => !m.IsDeleted));

        var totalMembers = households.Sum(h => h.Members.Count(m => !m.IsDeleted));
        var avgHouseholdSize = totalHouseholds > 0 ? Math.Round((double)totalMembers / totalHouseholds, 1) : 0;

        var budgets = await _context.HouseholdMonthlyBudgets
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .Select(b => b.Amount)
            .ToListAsync(cancellationToken);

        var avgIncome = budgets.Count > 0 ? budgets.Average() : 0m;

        var thisMonthNewHh = households.Count(h => h.CreatedAt >= startOfCurrentMonth);
        var lastMonthNewHh = households.Count(h => h.CreatedAt >= startOfLastMonth && h.CreatedAt < startOfCurrentMonth);
        var growthPct = lastMonthNewHh > 0
            ? ((double)(thisMonthNewHh - lastMonthNewHh) / lastMonthNewHh) * 100
            : (thisMonthNewHh > 0 ? 100.0 : 0.0);

        var totalUsers = await _context.Users.CountAsync(u => u.IsActive, cancellationToken);

        // Top Regions
        var topRegions = households
            .Where(h => h.City != null || h.Governorate != null)
            .GroupBy(h => h.City != null ? h.City.Name.Get() : h.Governorate!.Name.Get())
            .Select(g => new RegionHouseholdSummaryDto
            {
                Name = g.Key,
                Count = g.Count(),
                Percentage = totalHouseholds > 0 ? Math.Round((double)g.Count() / totalHouseholds * 100, 1) : 0
            })
            .OrderByDescending(r => r.Count)
            .Take(5)
            .ToList();

        // Size Distribution
        var size1To2 = 0;
        var size3To4 = 0;
        var size5Plus = 0;

        foreach (var hh in households)
        {
            var count = hh.Members.Count(m => !m.IsDeleted);
            if (count <= 2) size1To2++;
            else if (count <= 4) size3To4++;
            else size5Plus++;
        }

        var sizeDistribution = new List<HouseholdSizeCountDto>
        {
            new() { Size = "1-2 People", Count = size1To2 },
            new() { Size = "3-4 People", Count = size3To4 },
            new() { Size = "5+ People", Count = size5Plus }
        };

        return new HouseholdsSummaryDto
        {
            TotalHouseholds = totalHouseholds,
            ActiveHouseholds = activeHouseholds,
            AvgHouseholdSize = avgHouseholdSize,
            AvgHouseholdIncome = $"{avgIncome:N0} AED",
            GrowthRate = $"{(growthPct >= 0 ? "+" : "")}{growthPct:0.#}% MoM",
            TotalUsers = totalUsers,
            TopRegions = topRegions,
            SizeDistribution = sizeDistribution
        };
    }

    public async Task<MealPlansSummaryDto> GetMealPlansSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalMealPlans = await _context.MealPlans
            .AsNoTracking()
            .CountAsync(mp => !mp.IsDeleted, cancellationToken);

        var totalHouseholds = await _context.Households
            .AsNoTracking()
            .CountAsync(h => !h.IsDeleted, cancellationToken);

        var mealPlansPerHh = totalHouseholds > 0
            ? Math.Round((double)totalMealPlans / totalHouseholds, 1)
            : 0;

        return new MealPlansSummaryDto
        {
            MealPlansTotal = totalMealPlans,
            MealPlansPerHousehold = mealPlansPerHh
        };
    }

    public async Task<ShoppingTrendsDto> GetShoppingTrendsAsync(CancellationToken cancellationToken = default)
    {
        // 1. Most Bought Category
        var purchasedItems = await _context.ShoppingListItems
            .AsNoTracking()
            .Include(sli => sli.Category)
            .Include(sli => sli.Offer)
                .ThenInclude(o => o!.Supermarket)
            .Where(sli => sli.IsPurchased && !sli.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalPurchased = purchasedItems.Count;
        var mostBoughtCatGroup = purchasedItems
            .Where(sli => sli.Category != null)
            .GroupBy(sli => sli.CategoryId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        var mostBoughtCat = new CategoryShareDto
        {
            Name = mostBoughtCatGroup != null ? mostBoughtCatGroup.First().Category!.Name.Get() : string.Empty,
            Percentage = totalPurchased > 0 && mostBoughtCatGroup != null
                ? Math.Round((double)mostBoughtCatGroup.Count() / totalPurchased * 100, 1)
                : 0
        };

        // 2. Most Common Inventory Item
        var pantryItems = await _context.PantryItems
            .AsNoTracking()
            .Include(pi => pi.Category)
            .Where(pi => !pi.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalPantry = pantryItems.Count;
        var mostCommonPantryGroup = pantryItems
            .Where(pi => !string.IsNullOrWhiteSpace(pi.Name))
            .GroupBy(pi => pi.Name.Trim().ToLowerInvariant())
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        var mostCommonItem = new ItemShareDto
        {
            Name = mostCommonPantryGroup != null ? mostCommonPantryGroup.First().Name : string.Empty,
            Percentage = totalPantry > 0 && mostCommonPantryGroup != null
                ? Math.Round((double)mostCommonPantryGroup.Count() / totalPantry * 100, 1)
                : 0
        };

        // 3. Most Successful Supermarket
        var topSupermarketGroup = purchasedItems
            .Where(sli => sli.Offer?.Supermarket != null)
            .GroupBy(sli => sli.Offer!.SupermarketId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        var mostSuccessfulSupermarket = topSupermarketGroup != null
            ? topSupermarketGroup.First().Offer!.Supermarket!.Name.Get()
            : string.Empty;

        // 4. Allergy Rankings (strictly from member preferences belonging to Allergies category)
        var membersWithPreferences = await _context.HouseholdMembers
            .AsNoTracking()
            .Include(m => m.Preferences)
                .ThenInclude(p => p.Category)
            .Where(m => !m.IsDeleted)
            .ToListAsync(cancellationToken);

        var allergyPreferences = membersWithPreferences
            .SelectMany(m => m.Preferences)
            .Where(p => p.Category != null && (p.Category.Name.Get("en").Contains("Allerg", StringComparison.OrdinalIgnoreCase) || p.Category.Name.Get("ar").Contains("حساسية", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var totalAllergies = allergyPreferences.Count;
        var allergyRanking = new List<AllergyRankingDto>();

        if (totalAllergies > 0)
        {
            allergyRanking = allergyPreferences
                .GroupBy(p => p.Id)
                .Select(g => new AllergyRankingDto
                {
                    Allergen = g.First().Name.Get(),
                    Percentage = Math.Round((double)g.Count() / totalAllergies * 100, 1)
                })
                .OrderByDescending(a => a.Percentage)
                .Take(5)
                .ToList();
        }

        return new ShoppingTrendsDto
        {
            MostBoughtCategory = mostBoughtCat,
            MostCommonInventoryItem = mostCommonItem,
            MostSuccessfulSupermarket = mostSuccessfulSupermarket,
            AllergyRanking = allergyRanking
        };
    }

    public async Task<UserDemographicsDto> GetUserDemographicsAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);

        var householders = await _context.HouseholdMembers
            .AsNoTracking()
            .Include(m => m.User)
            .Where(m => !m.IsDeleted && (m.Role == Roles.HouseholdManager || m.Role == "HouseholdManager"))
            .ToListAsync(cancellationToken);

        // 1. Avg Age Users
        var userAges = users
            .Select(u => CalculateAge(u.BirthDate))
            .Where(age => age > 0)
            .ToList();

        var avgAgeUsers = userAges.Count > 0 ? Math.Round(userAges.Average(), 0) : 0;

        // 2. Avg Age Householders
        var householderAges = new List<int>();
        foreach (var hh in householders)
        {
            if (hh.DateOfBirth.HasValue)
            {
                var age = CalculateAge(hh.DateOfBirth.Value);
                if (age > 0) householderAges.Add(age);
            }
            else if (hh.User != null)
            {
                var age = CalculateAge(hh.User.BirthDate);
                if (age > 0) householderAges.Add(age);
            }
        }

        var avgAgeHouseholders = householderAges.Count > 0 ? Math.Round(householderAges.Average(), 0) : 0;

        // 3. Gender Split Users
        var femaleUsers = users.Count(u => u.Gender == Gender.Female);
        var maleUsers = users.Count(u => u.Gender == Gender.Male);
        var totalUserGender = femaleUsers + maleUsers;

        var genderSplitUsers = new List<GenderSplitDto>
        {
            new() { Gender = "Female", Percentage = totalUserGender > 0 ? Math.Round((double)femaleUsers / totalUserGender * 100, 1) : 0 },
            new() { Gender = "Male", Percentage = totalUserGender > 0 ? Math.Round((double)maleUsers / totalUserGender * 100, 1) : 0 }
        };

        // 4. Gender Split Householders
        var femaleHh = householders.Count(h => h.Gender == Gender.Female || (h.User != null && h.User.Gender == Gender.Female));
        var maleHh = householders.Count(h => h.Gender == Gender.Male || (h.User != null && h.User.Gender == Gender.Male));
        var totalHhGender = femaleHh + maleHh;

        var genderSplitHouseholders = new List<GenderSplitDto>
        {
            new() { Gender = "Female", Percentage = totalHhGender > 0 ? Math.Round((double)femaleHh / totalHhGender * 100, 1) : 0 },
            new() { Gender = "Male", Percentage = totalHhGender > 0 ? Math.Round((double)maleHh / totalHhGender * 100, 1) : 0 }
        };

        return new UserDemographicsDto
        {
            AvgAgeHouseholders = avgAgeHouseholders,
            AvgAgeUsers = avgAgeUsers,
            GenderSplitHouseholders = genderSplitHouseholders,
            GenderSplitUsers = genderSplitUsers
        };
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
            age--;
        return Math.Max(age, 0);
    }
}
