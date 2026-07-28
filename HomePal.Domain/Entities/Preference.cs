namespace HomePal.Domain.Entities;

public class Preference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<HouseholdMember> Members { get; set; } = new List<HouseholdMember>();
}
