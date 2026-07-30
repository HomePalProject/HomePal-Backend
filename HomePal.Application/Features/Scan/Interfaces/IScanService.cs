using HomePal.Application.Features.Scan.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Scan.Interfaces;

public interface IScanService
{
    Task<Result<ScanResponse>> ScanAsync(ScanImageRequest request, CancellationToken cancellationToken = default);
}
