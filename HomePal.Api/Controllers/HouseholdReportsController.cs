using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/reports")]
public class HouseholdReportsController : BaseApiController
{
    private readonly IHouseholdReportService _reportService;

    public HouseholdReportsController(IHouseholdReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get the comprehensive household overview analytics and KPIs report
    /// </summary>
    [HttpGet("household-overview")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdOverviewReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHouseholdOverview(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetHouseholdOverviewAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
}
