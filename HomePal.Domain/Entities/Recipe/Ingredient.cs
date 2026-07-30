using HomePal.Domain.Enums.RecipeEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePal.Domain.Entities.Recipe
{
    public class Ingredient
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public MeasurementUnit DefaultUnit { get; set; }

        public IngredientCategory Category { get; set; }

        public ICollection<RecipeIngredient> Recipes { get; set; }
            = new List<RecipeIngredient>();
    }
}
