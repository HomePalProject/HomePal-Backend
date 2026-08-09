using HomePal.Application.Common.Extensions;
using HomePal.Application.Common.Interfaces;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using Microsoft.Data.SqlTypes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomePal.Infrastructure.AI.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly AgentOptions _options;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IOptions<AgentOptions> options,
        ILogger<EmbeddingService> logger)
    {
        _embeddingGenerator = embeddingGenerator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<float[]?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            var options = new EmbeddingGenerationOptions
            {
                Dimensions = _options.Dimensions > 0 ? _options.Dimensions : 1536
            };
            
            var result = await _embeddingGenerator.GenerateAsync([text], options, cancellationToken: cancellationToken);
            var vector = result.FirstOrDefault()?.Vector.ToArray();
            return vector;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating text embedding vector.");
            return null;
        }
    }

    public async Task<SqlVector<float>?> GenerateSqlVectorAsync(string text, CancellationToken cancellationToken = default)
    {
        var floatArray = await GenerateEmbeddingAsync(text, cancellationToken);
        return floatArray.ToSqlVector();
    }
}
