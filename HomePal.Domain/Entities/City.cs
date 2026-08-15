using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class City : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public Guid GovernorateId { get; set; }
    public Governorate Governorate { get; set; } = null!;
    public string GovernorateCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
