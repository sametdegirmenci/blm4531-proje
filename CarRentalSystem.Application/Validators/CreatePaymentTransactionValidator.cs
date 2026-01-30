using FluentValidation;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Domain.Enums;
using System;

namespace CarRentalSystem.Application.Validators;

public class CreatePaymentTransactionValidator : AbstractValidator<CreatePaymentTransactionDto>
{
    public CreatePaymentTransactionValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.TransactionReference).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PaymentMethod).IsEnumName(typeof(PaymentMethod), caseSensitive: false);
    }
}
