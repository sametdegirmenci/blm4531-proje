namespace CarRentalSystem.Application.DTOs;

public record VehicleRentalReportDto(
    int VehicleId,
    string Brand,
    string Model,
    int RentalCount,
    decimal TotalRevenue
);

public record MonthlyRevenueReportDto(
    int Year,
    int Month,
    decimal TotalRevenue,
    int TotalReservations
);
