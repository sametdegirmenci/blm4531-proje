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
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentTransactionDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _paymentService.ProcessPaymentAsync(userId, dto);
        return Ok(ApiResponse<PaymentTransactionDto>.SuccessResponse(result, "Payment processed successfully"));
    }

    [HttpGet("reservation/{reservationId}")]
    public async Task<IActionResult> GetReservationPayments(int reservationId)
    {
        // Ideally we should check if the user owns the reservation or is admin
        var result = await _paymentService.GetReservationPaymentsAsync(reservationId);
        return Ok(ApiResponse<IEnumerable<PaymentTransactionDto>>.SuccessResponse(result, "Payments retrieved successfully"));
    }

    [HttpGet("my-payments")]
    public async Task<IActionResult> GetMyPayments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _paymentService.GetUserPaymentsAsync(userId);
        return Ok(ApiResponse<IEnumerable<PaymentTransactionDto>>.SuccessResponse(result, "Your payments retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);
        return Ok(ApiResponse<PaymentTransactionDto>.SuccessResponse(result, "Payment details retrieved successfully"));
    }
}
