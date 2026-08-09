using Microsoft.Data.SqlTypes;

namespace HomePal.Application.Common.Interfaces;

public interface IEmbeddingService
{
    Task<float[]?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<SqlVector<float>?> GenerateSqlVectorAsync(string text, CancellationToken cancellationToken = default);
}
