using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class ProductCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CanonicalProduct> Products { get; set; } = new List<CanonicalProduct>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
