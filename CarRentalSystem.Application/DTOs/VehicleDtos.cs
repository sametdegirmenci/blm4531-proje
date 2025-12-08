namespace CarRentalSystem.Application.DTOs;

public record VehicleDto(
    int VehicleId,
    string Brand,
    string Model,
    int Year,
    decimal PricePerDay,
    bool IsAvailable,
    string? ImageUrl,
    string? LicensePlate,
    DateTime CreatedAt
);

public record CreateVehicleDto(
    string Brand,
    string Model,
    int Year,
    decimal PricePerDay,
    string? ImageUrl,
    string? LicensePlate
);

public record UpdateVehicleDto(
    string Brand,
    string Model,
    int Year,
    decimal PricePerDay,
    bool IsAvailable,
    string? ImageUrl,
    string? LicensePlate
);
