using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class Supermarket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? WebsiteUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
