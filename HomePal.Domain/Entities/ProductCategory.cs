using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class ProductCategory : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public string? ImagePath { get; set; }

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
