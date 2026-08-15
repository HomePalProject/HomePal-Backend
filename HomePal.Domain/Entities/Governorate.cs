using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class Governorate : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public List<LocalizedItem> Name { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<City> Cities { get; set; } = new List<City>();
}
