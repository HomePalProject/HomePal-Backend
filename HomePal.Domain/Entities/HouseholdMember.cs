using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class HouseholdMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HouseholdId { get; set; }
    public Guid? UserId { get; set; } // Nullable for offline / non-registered members
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Male;
    public DateOnly? DateOfBirth { get; set; }
    public string Role { get; set; } = string.Empty; // HouseholdManager or HouseholdMember
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Household Household { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}
