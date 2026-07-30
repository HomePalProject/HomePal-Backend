using HomePal.Application.Features.Scan.DTOs;
using HomePal.Application.Features.Scan.Interfaces;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Scan.Services;

public class ScanService : IScanService
{
    public Task<Result<ScanResponse>> ScanAsync(ScanImageRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.Image == null || request.Image.Length == 0)
        {
            return Task.FromResult(Result<ScanResponse>.Fail(ErrorMessages.Scan.NoImageUploaded, ResultStatus.BadRequest));
        }

        var scanResult = new ScanResponse
        {
            RawText = $"SCANNED: {request.Image.FileName} (Size: {request.Image.Length} bytes)",
            DetectedName = "Organic Whole Milk",
            DetectedCategory = "Dairy",
            SuggestedQuantity = 1.0m,
            SuggestedUnit = "Gallon",
            Confidence = 0.96
        };

        return Task.FromResult(Result<ScanResponse>.Ok(scanResult, SuccessMessages.Scan.ScanSuccess));
    }
}
