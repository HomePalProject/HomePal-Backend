using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class MeasuringUnit : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem> Symbol { get; set; } = new();

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
