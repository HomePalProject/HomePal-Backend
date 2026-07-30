using HomePal.Domain.Enums.RecipeEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePal.Domain.Entities.Recipe
{
    public class Recipe
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DifficultyLevel Difficulty { get; set; }

        public TimeSpan TimeToMake { get; set; }

        public int Servings { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<RecipeIngredient> Ingredients { get; set; }
            = new List<RecipeIngredient>();

        public ICollection<RecipeStep> Steps { get; set; }
            = new List<RecipeStep>();
    }
}
