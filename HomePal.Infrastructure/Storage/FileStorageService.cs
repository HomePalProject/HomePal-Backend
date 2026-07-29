using HomePal.Application.Common.Interfaces;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HomePal.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IWebHostEnvironment _environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException(ErrorMessages.Auth.InvalidImageFile);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException(ErrorMessages.Auth.ImageSizeExceeded);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(ErrorMessages.Auth.InvalidImageFile);
        }

        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        }

        var uploadsFolder = Path.Combine(webRootPath, "uploads", folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var relativePath = $"/uploads/{folderName}/{uniqueFileName}";
        return relativePath;
    }

    public Task DeleteFileAsync(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        }

        var trimmedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(webRootPath, trimmedPath);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch
            {
                // Ignore file deletion errors to avoid blocking business logic
            }
        }

        return Task.CompletedTask;
    }
}
