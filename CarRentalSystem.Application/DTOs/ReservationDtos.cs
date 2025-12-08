namespace CarRentalSystem.Application.DTOs;

public record ReservationDto(
    int ReservationId,
    int UserId,
    int VehicleId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt,
    VehicleDto? Vehicle = null,
    UserDto? User = null
);

public record CreateReservationDto(
    int VehicleId,
    DateTime StartDate,
    DateTime EndDate
);

public record UpdateReservationDto(
    DateTime StartDate,
    DateTime EndDate
);
