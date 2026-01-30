using System.Collections.Generic;
using System.Threading.Tasks;
using CarRentalSystem.Application.DTOs;

namespace CarRentalSystem.Application.Services;

public interface IPaymentService
{
    Task<PaymentTransactionDto> ProcessPaymentAsync(int userId, CreatePaymentTransactionDto dto);
    Task<IEnumerable<PaymentTransactionDto>> GetReservationPaymentsAsync(int reservationId);
    Task<IEnumerable<PaymentTransactionDto>> GetUserPaymentsAsync(int userId);
    Task<PaymentTransactionDto> GetPaymentByIdAsync(int id);
}
