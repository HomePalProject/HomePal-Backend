using HomePal.Application.Features.Scan.DTOs;
using HomePal.Application.Features.Scan.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/scan")]
public class ScanController : BaseApiController
{
    private readonly IScanService _scanService;

    public ScanController(IScanService scanService)
    {
        _scanService = scanService;
    }

    /// <summary>
    /// Upload image for scanning (multipart/form-data)
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<ScanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Scan([FromForm] ScanImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _scanService.ScanAsync(request, cancellationToken);
        return HandleResult(result);
    }
}
