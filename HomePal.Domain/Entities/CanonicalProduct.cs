using HomePal.Domain.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;

namespace HomePal.Domain.Entities;

public class CanonicalProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }

    public Guid? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    public SqlVector<float>? Embedding { get; set; }
    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
