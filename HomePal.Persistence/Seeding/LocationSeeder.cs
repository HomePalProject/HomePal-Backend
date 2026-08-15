using System.Text.Json;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomePal.Persistence.Seeding;

public static class LocationSeeder
{
    public static async Task SeedLocationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("LocationSeeder");

        if (await dbContext.Governorates.AnyAsync())
        {
            logger.LogInformation("Governorates and cities are already seeded.");
            return;
        }

        var govJsonPath = FindJsonPath("governorates_with_coordinates.json");
        var cityJsonPath = FindJsonPath("cities_with_coordinates.json");

        if (govJsonPath == null || !File.Exists(govJsonPath) || cityJsonPath == null || !File.Exists(cityJsonPath))
        {
            logger.LogWarning("Location JSON files not found. Skipping location seeding. GovPath: {GovPath}, CityPath: {CityPath}", govJsonPath, cityJsonPath);
            return;
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var govJson = await File.ReadAllTextAsync(govJsonPath);
        var govDtos = JsonSerializer.Deserialize<List<GovernorateSeedDto>>(govJson, jsonOptions);

        var cityJson = await File.ReadAllTextAsync(cityJsonPath);
        var cityDtos = JsonSerializer.Deserialize<List<CitySeedDto>>(cityJson, jsonOptions);

        if (govDtos == null || govDtos.Count == 0 || cityDtos == null || cityDtos.Count == 0)
        {
            logger.LogWarning("Failed to parse governorate or city JSON files.");
            return;
        }

        logger.LogInformation("Seeding {GovCount} governorates and {CityCount} cities...", govDtos.Count, cityDtos.Count);

        var govMap = new Dictionary<string, Governorate>(StringComparer.OrdinalIgnoreCase);
        var governorates = new List<Governorate>();

        foreach (var dto in govDtos)
        {
            var gov = new Governorate
            {
                Id = Guid.NewGuid(),
                Code = dto.Code.Trim().ToUpperInvariant(),
                Name = new List<LocalizedItem>
                {
                    new("ar", dto.Name?.Trim() ?? string.Empty),
                    new("en", dto.NameEn?.Trim() ?? string.Empty)
                },
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CreatedAt = DateTime.UtcNow
            };

            govMap[gov.Code] = gov;
            governorates.Add(gov);
        }

        var cities = new List<City>();
        foreach (var dto in cityDtos)
        {
            var govCode = dto.GovernorateCode?.Trim().ToUpperInvariant() ?? string.Empty;
            if (!govMap.TryGetValue(govCode, out var parentGov))
            {
                logger.LogWarning("Governorate with code {Code} not found for city {CityName}", govCode, dto.Name);
                continue;
            }

            var city = new City
            {
                Id = Guid.NewGuid(),
                Name = new List<LocalizedItem>
                {
                    new("ar", dto.Name?.Trim() ?? string.Empty),
                    new("en", dto.NameEn?.Trim() ?? string.Empty)
                },
                GovernorateId = parentGov.Id,
                GovernorateCode = parentGov.Code,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CreatedAt = DateTime.UtcNow
            };

            cities.Add(city);
        }

        await dbContext.Governorates.AddRangeAsync(governorates);
        await dbContext.Cities.AddRangeAsync(cities);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {GovCount} governorates and {CityCount} cities.", governorates.Count, cities.Count);
    }

    private static string? FindJsonPath(string fileName)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Seeding", fileName),
            Path.Combine(AppContext.BaseDirectory, fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "HomePal.Persistence", "Seeding", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "HomePal.Persistence", "Seeding", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Seeding", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), fileName)
        };

        return candidates.FirstOrDefault(File.Exists);
    }

    private class GovernorateSeedDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    private class CitySeedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string GovernorateCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
