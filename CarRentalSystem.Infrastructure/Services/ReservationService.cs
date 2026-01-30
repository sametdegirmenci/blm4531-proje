using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Exceptions;
using CarRentalSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(
        IReservationRepository reservationRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<ReservationService> logger)
    {
        _reservationRepository = reservationRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReservationDto> CreateReservationAsync(int userId, CreateReservationDto dto)
    {
        _logger.LogInformation("Attempting to create reservation for UserId: {UserId} with DTO: {@CreateReservationDto}", userId, dto);

        // Validate user exists
        if (!await _userRepository.ExistsAsync(userId))
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
        if (vehicle == null)
        {
            throw new NotFoundException(nameof(Vehicle), dto.VehicleId);
        }

        // Validate dates
        if (dto.EndDate <= dto.StartDate)
        {
            throw new ValidationException("End date must be after start date.");
        }

        if (dto.StartDate < DateTime.UtcNow.Date)
        {
            throw new ValidationException("Start date cannot be in the past.");
        }

        // Check for conflicting reservations
        var hasConflict = await _reservationRepository.HasConflictingReservationAsync(dto.VehicleId, dto.StartDate, dto.EndDate);
        _logger.LogInformation("Conflict check for VehicleId: {VehicleId} from {StartDate} to {EndDate}. Result: {HasConflict}", dto.VehicleId, dto.StartDate, dto.EndDate, hasConflict);

        if (hasConflict)
        {
            throw new ConflictException("Vehicle is already reserved for the selected dates.");
        }

        // Calculate total price
        var rentalDays = (dto.EndDate - dto.StartDate).Days;
        if (rentalDays < 1)
        {
            throw new ValidationException("Reservation must be for at least one day.");
        }

        var totalPrice = vehicle.PricePerDay * rentalDays;

        // Create reservation
        var reservation = new Reservation
        {
            UserId = userId,
            VehicleId = dto.VehicleId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalPrice = totalPrice,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        reservation = await _reservationRepository.AddAsync(reservation);

        // Reload with navigation properties
        reservation = await _reservationRepository.GetByIdAsync(reservation.ReservationId);

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> GetReservationByIdAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
    {
        var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto> UpdateReservationAsync(int id, int userId, UpdateReservationDto dto)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        // Verify user owns the reservation
        if (reservation.UserId != userId)
        {
            throw new UnauthorizedException("You can only update your own reservations.");
        }

        // Only allow updates for Pending reservations
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ValidationException("Only pending reservations can be updated.");
        }

        // Validate dates
        if (dto.EndDate <= dto.StartDate)
        {
            throw new ValidationException("End date must be after start date.");
        }

        if (dto.StartDate < DateTime.UtcNow.Date)
        {
            throw new ValidationException("Start date cannot be in the past.");
        }

        // Check for conflicts (excluding current reservation)
        if (await _reservationRepository.HasConflictingReservationAsync(
            reservation.VehicleId, dto.StartDate, dto.EndDate, id))
        {
            throw new ConflictException("Vehicle is already reserved for the selected dates.");
        }

        // Get vehicle to recalculate price
        var vehicle = await _vehicleRepository.GetByIdAsync(reservation.VehicleId);
        var rentalDays = (dto.EndDate - dto.StartDate).Days;
        var totalPrice = vehicle.PricePerDay * rentalDays;

        // Update reservation
        reservation.StartDate = dto.StartDate;
        reservation.EndDate = dto.EndDate;
        reservation.TotalPrice = totalPrice;

        await _reservationRepository.UpdateAsync(reservation);

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task CancelReservationAsync(int id, int userId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        // Verify user owns the reservation
        if (reservation.UserId != userId)
        {
            throw new UnauthorizedException("You can only cancel your own reservations.");
        }

        // Only allow cancellation of Pending reservations
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ValidationException("Only pending reservations can be cancelled.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _reservationRepository.UpdateAsync(reservation);
    }

    public async Task CancelReservationPaymentAsync(int id, int userId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        // Verify user owns the reservation
        if (reservation.UserId != userId)
        {
            throw new UnauthorizedException("You can only cancel payment for your own reservations.");
        }

        // Only allow if Pending
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ValidationException("Only pending reservations can be rejected via payment cancellation.");
        }

        // Requirement: Set status to Rejected
        reservation.Status = ReservationStatus.Rejected;
        await _reservationRepository.UpdateAsync(reservation);
    }

    public async Task<ReservationDto> ConfirmReservationAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        // Only allow confirmation of Pending reservations
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ValidationException("Only pending reservations can be confirmed.");
        }

        reservation.Status = ReservationStatus.Confirmed;
        await _reservationRepository.UpdateAsync(reservation);

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> RejectReservationAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null)
        {
            throw new NotFoundException(nameof(Reservation), id);
        }

        // Only allow rejection of Pending reservations
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ValidationException("Only pending reservations can be rejected.");
        }

        reservation.Status = ReservationStatus.Rejected;
        await _reservationRepository.UpdateAsync(reservation);

        return _mapper.Map<ReservationDto>(reservation);
    }
}
