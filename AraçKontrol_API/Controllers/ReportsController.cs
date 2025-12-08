using CarRentalSystem.Application.Common;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AraçKontrol_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    /// <summary>
    /// Get most rented vehicles (Admin only)
    /// </summary>
    [HttpGet("most-rented")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VehicleRentalReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMostRented([FromQuery] int topCount = 10)
    {
        var report = await _reportingService.GetMostRentedVehiclesAsync(topCount);
        var response = new ApiResponse<IEnumerable<VehicleRentalReportDto>>(report);
        return Ok(response);
    }

    /// <summary>
    /// Get monthly revenue report (Admin only)
    /// </summary>
    [HttpGet("monthly-revenue/{year}/{month}")]
    [ProducesResponseType(typeof(ApiResponse<MonthlyRevenueReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlyRevenue(int year, int month)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest(new ApiResponse<object>(null, "Month must be between 1 and 12", false));
        }

        var report = await _reportingService.GetMonthlyRevenueAsync(year, month);
        var response = new ApiResponse<MonthlyRevenueReportDto>(report);
        return Ok(response);
    }

    /// <summary>
    /// Get reservations by date range (Admin only)
    /// </summary>
    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate >= endDate)
        {
            return BadRequest(new ApiResponse<object>(null, "Start date must be before end date", false));
        }

        var reservations = await _reportingService.GetReservationsByDateRangeAsync(startDate, endDate);
        var response = new ApiResponse<IEnumerable<ReservationDto>>(reservations);
        return Ok(response);
    }
}
