using CarRentalSystem.Application.Common;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AraçKontrol_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Get all reservations (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        var response = new ApiResponse<IEnumerable<ReservationDto>>(reservations);
        return Ok(response);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        var response = new ApiResponse<ReservationDto>(reservation);
        return Ok(response);
    }

    /// <summary>
    /// Get reservations for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var reservations = await _reservationService.GetUserReservationsAsync(userId);
        var response = new ApiResponse<IEnumerable<ReservationDto>>(reservations);
        return Ok(response);
    }

    /// <summary>
    /// Get reservations for the current logged-in user
    /// </summary>
    [HttpGet("my-reservations")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReservations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var reservations = await _reservationService.GetUserReservationsAsync(userId);
        var response = new ApiResponse<IEnumerable<ReservationDto>>(reservations);
        return Ok(response);
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReservationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var reservation = await _reservationService.CreateReservationAsync(userId, dto);
        var response = new ApiResponse<ReservationDto>(reservation, "Reservation created successfully");
        return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, response);
    }

    /// <summary>
    /// Update a reservation
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var reservation = await _reservationService.UpdateReservationAsync(id, userId, dto);
        var response = new ApiResponse<ReservationDto>(reservation, "Reservation updated successfully");
        return Ok(response);
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpDelete("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _reservationService.CancelReservationAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Confirm a reservation (Admin only)
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm(int id)
    {
        var reservation = await _reservationService.ConfirmReservationAsync(id);
        var response = new ApiResponse<ReservationDto>(reservation, "Reservation confirmed successfully");
        return Ok(response);
    }

    /// <summary>
    /// Reject a reservation (Admin only)
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(int id)
    {
        var reservation = await _reservationService.RejectReservationAsync(id);
        var response = new ApiResponse<ReservationDto>(reservation, "Reservation rejected successfully");
        return Ok(response);
    }
}
