using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services;

namespace Backend.Controllers;

[Authorize(Roles = "Admin,Employee")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardReport()
    {
        try
        {
            var report = await _reportService.GetDashboardReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error generating dashboard report: {ex.Message}" });
        }
    }

    [HttpGet("rentals/statistics")]
    public async Task<IActionResult> GetRentalStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var statistics = await _reportService.GetRentalStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error generating rental statistics: {ex.Message}" });
        }
    }

    [HttpGet("vehicles/utilization")]
    public async Task<IActionResult> GetVehicleUtilization([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var utilization = await _reportService.GetVehicleUtilizationReportAsync(startDate, endDate);
            return Ok(utilization);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error generating vehicle utilization report: {ex.Message}" });
        }
    }

    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int months = 12)
    {
        try
        {
            var revenue = await _reportService.GetMonthlyRevenueReportAsync(months);
            return Ok(revenue);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error generating monthly revenue report: {ex.Message}" });
        }
    }
}
