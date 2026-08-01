using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class PreferenceCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
}


