using HomePal.Domain.Enums.RecipeEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePal.Domain.Entities.Recipe
{
    public class RecipeIngredient
    {
        public Guid RecipeId { get; set; }

        public Recipe Recipe { get; set; } = null!;

        public Guid IngredientId { get; set; }

        public Ingredient Ingredient { get; set; } = null!;

        public decimal Amount { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
