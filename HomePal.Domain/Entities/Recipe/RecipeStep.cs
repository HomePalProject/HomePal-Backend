using System;
using System.Collections.Generic;
using System.Text;

namespace HomePal.Domain.Entities.Recipe
{
    public class RecipeStep
    {
        public Guid Id { get; set; }

        public Guid RecipeId { get; set; }

        public Recipe Recipe { get; set; } = null!;

        public int StepOrder { get; set; }

        public string Description { get; set; } = null!;
    }
}
