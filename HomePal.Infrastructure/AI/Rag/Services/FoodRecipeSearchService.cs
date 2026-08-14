using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Infrastructure.AI.Rag.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HomePal.Infrastructure.AI.Rag.Services;

public class FoodRecipeSearchService : IFoodRecipeSearchService
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly MongoRagOptions _options;
    private readonly ILogger<FoodRecipeSearchService> _logger;

    public FoodRecipeSearchService(
        IMongoClient mongoClient,
        IOptions<MongoRagOptions> options,
        ILogger<FoodRecipeSearchService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var database = mongoClient.GetDatabase(_options.DatabaseName);
        _collection = database.GetCollection<BsonDocument>(_options.FoodRecipesCollectionName);
    }

    public async Task<IReadOnlyList<string>> SearchAsync(
        string query,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            _logger.LogWarning("Search query cannot be empty for recipe vector search.");
            return Array.Empty<string>();
        }

        var effectiveLimit = limit > 0 ? limit : _options.DefaultLimit;

        try
        {
            var pipeline = new[]
            {
                new BsonDocument(
                    "$vectorSearch",
                    new BsonDocument
                    {
                        { "index", _options.IndexName },
                        { "path", _options.EmbeddingPath },
                        {
                            "query",
                            new BsonDocument
                            {
                                { "text", query }
                            }
                        },
                        { "model", _options.EmbeddingModel },
                        { "numCandidates", _options.NumCandidates },
                        { "limit", effectiveLimit }
                    }),

                new BsonDocument(
                    "$set",
                    new BsonDocument
                    {
                        {
                            "score",
                            new BsonDocument(
                                "$meta",
                                "vectorSearchScore")
                        }
                    })
            };

            var results = await _collection
                .Aggregate<BsonDocument>(pipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken);

            return results.Select(r => r.ToJson()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during food recipe vector search for query '{Query}'.", query);
            return Array.Empty<string>();
        }
    }
}
