namespace HomePal.Infrastructure.AI.Rag.Options;

public class MongoRagOptions
{
    public const string SectionName = "MongoRagOptions";

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = "HomePal";

    public string FoodRecipesCollectionName { get; set; } = "FoodRecipesFixed";

    public string IngredientsCollectionName { get; set; } = "Ingrediants";

    public string IndexName { get; set; } = "autoembed_index";

    public string EmbeddingPath { get; set; } = "embeddingText";

    public string EmbeddingModel { get; set; } = "voyage-4";

    public int DefaultLimit { get; set; } = 5;

    public int NumCandidates { get; set; } = 50;
}
