using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class MeasurementUnit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Symbol { get; set; }
    public List<LocalizedItem>? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
