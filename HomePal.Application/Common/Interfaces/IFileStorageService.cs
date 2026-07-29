using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string? relativePath);
}
