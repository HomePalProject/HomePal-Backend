using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class MeasuringUnit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem> Symbol { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
