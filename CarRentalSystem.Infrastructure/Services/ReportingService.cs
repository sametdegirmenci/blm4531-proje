using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReportingService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VehicleRentalReportDto>> GetMostRentedVehiclesAsync(int topCount = 10)
    {
        var report = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Completed)
            .GroupBy(r => new { r.VehicleId, r.Vehicle.Brand, r.Vehicle.Model })
            .Select(g => new VehicleRentalReportDto(
                g.Key.VehicleId,
                g.Key.Brand,
                g.Key.Model,
                g.Count(),
                g.Sum(r => r.TotalPrice)
            ))
            .OrderByDescending(r => r.RentalCount)
            .Take(topCount)
            .ToListAsync();

        return report;
    }

    public async Task<MonthlyRevenueReportDto> GetMonthlyRevenueAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var completedReservations = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Completed &&
                       r.CreatedAt >= startDate &&
                       r.CreatedAt < endDate)
            .ToListAsync();

        var totalRevenue = completedReservations.Sum(r => r.TotalPrice);
        var totalReservations = completedReservations.Count;

        return new MonthlyRevenueReportDto(year, month, totalRevenue, totalReservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var reservations = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate >= startDate && r.EndDate <= endDate)
            .OrderBy(r => r.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }
}
