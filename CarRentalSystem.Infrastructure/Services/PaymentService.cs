using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Exceptions;
using CarRentalSystem.Domain.Interfaces;

namespace CarRentalSystem.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentTransactionRepository _paymentRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;

    public PaymentService(
        IPaymentTransactionRepository paymentRepository,
        IReservationRepository reservationRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }

    public async Task<PaymentTransactionDto> ProcessPaymentAsync(int userId, CreatePaymentTransactionDto dto)
    {
        var reservation = await _reservationRepository.GetByIdAsync(dto.ReservationId);
        if (reservation == null)
            throw new NotFoundException(nameof(Reservation), dto.ReservationId);

        if (reservation.UserId != userId)
            throw new UnauthorizedException("You can only pay for your own reservations.");

        var payment = _mapper.Map<PaymentTransaction>(dto);
        payment.UserId = userId;
        
        // In a real app, here we would integrate with Stripe/PayPal.
        // For now, we simulate a successful transaction.
        payment.Status = PaymentStatus.Completed; 
        
        var created = await _paymentRepository.AddAsync(payment);
        
        // Refetch to get related entities for DTO
        var fullEntity = await _paymentRepository.GetByIdAsync(created.Id);
        return _mapper.Map<PaymentTransactionDto>(fullEntity);
    }

    public async Task<IEnumerable<PaymentTransactionDto>> GetReservationPaymentsAsync(int reservationId)
    {
        var payments = await _paymentRepository.GetByReservationIdAsync(reservationId);
        return _mapper.Map<IEnumerable<PaymentTransactionDto>>(payments);
    }

    public async Task<IEnumerable<PaymentTransactionDto>> GetUserPaymentsAsync(int userId)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<PaymentTransactionDto>>(payments);
    }

    public async Task<PaymentTransactionDto> GetPaymentByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            throw new NotFoundException(nameof(PaymentTransaction), id);

        return _mapper.Map<PaymentTransactionDto>(payment);
    }
}
