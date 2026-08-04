using Microsoft.Data.SqlTypes;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class VectorMapperExtensions
{
    public static SqlVector<float>? ToSqlVector(this float[]? values)
    {
        if (values == null || values.Length == 0) return null;
        return new SqlVector<float>(new ReadOnlyMemory<float>(values));
    }
}
