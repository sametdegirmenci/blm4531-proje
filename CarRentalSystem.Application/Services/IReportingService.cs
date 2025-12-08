using CarRentalSystem.Application.DTOs;

namespace CarRentalSystem.Application.Services;

public interface IReportingService
{
    Task<IEnumerable<VehicleRentalReportDto>> GetMostRentedVehiclesAsync(int topCount = 10);
    Task<MonthlyRevenueReportDto> GetMonthlyRevenueAsync(int year, int month);
    Task<IEnumerable<ReservationDto>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
