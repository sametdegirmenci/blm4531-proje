using System;

namespace CarRentalSystem.Application.DTOs;

public record PaymentTransactionDto(
    int Id,
    int ReservationId,
    int UserId,
    decimal Amount,
    DateTime TransactionDate,
    string PaymentMethod,
    string Status,
    string TransactionReference
);

public record CreatePaymentTransactionDto(
    int ReservationId,
    decimal Amount,
    string PaymentMethod,
    string TransactionReference
);
