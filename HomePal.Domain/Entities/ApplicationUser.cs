using HomePal.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HomePal.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Male;
    public DateOnly BirthDate { get; set; }
    public Guid? GovernorateId { get; set; }
    public Governorate? Governorate { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsProfileComplete { get; set; } = true;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

