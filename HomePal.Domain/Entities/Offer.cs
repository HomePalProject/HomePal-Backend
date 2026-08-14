using HomePal.Domain.Common;
using Microsoft.Data.SqlTypes;

namespace HomePal.Domain.Entities;

public class Offer : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }

    public double Quantity { get; set; }
    public Guid? UnitId { get; set; }
    public MeasuringUnit? Unit { get; set; }

    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public Guid? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    public string? ImagePath { get; set; }

    public Guid SupermarketId { get; set; }
    public Supermarket Supermarket { get; set; } = null!;

    public SqlVector<float>? Embedding { get; set; }
    public bool IsVerified { get; set; } = true;
}
