using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VehicleRepository> _logger;

    public VehicleRepository(ApplicationDbContext context, ILogger<VehicleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Vehicle> GetByIdAsync(int id)
    {
        return await _context.Vehicles
            .Include(v => v.Reservations)
            .FirstOrDefaultAsync(v => v.VehicleId == id);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.Reservations)
            .ToListAsync();
        _logger.LogInformation("Fetched {VehicleCount} vehicles from the database.", vehicles.Count);
        return vehicles;
    }

    public async Task<Vehicle> AddAsync(Vehicle entity)
    {
        await _context.Vehicles.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Vehicle entity)
    {
        _context.Vehicles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Vehicles.AnyAsync(v => v.VehicleId == id);
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _context.Vehicles
            .Where(v => v.IsAvailable)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Vehicles
            .Where(v => v.IsAvailable)
            .Where(v => !v.Reservations.Any(r =>
                (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed) &&
                ((startDate >= r.StartDate && startDate < r.EndDate) ||
                 (endDate > r.StartDate && endDate <= r.EndDate) ||
                 (startDate <= r.StartDate && endDate >= r.EndDate))))
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByBrandAsync(string brand)
    {
        return await _context.Vehicles
            .Where(v => v.Brand.ToLower() == brand.ToLower())
            .ToListAsync();
    }

    public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null || !vehicle.IsAvailable)
            return false;

        // Check if there are any active reservations that conflict with the requested dates
        var hasConflict = await _context.Reservations
            .AnyAsync(r => r.VehicleId == vehicleId &&
                          (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed) &&
                          ((startDate >= r.StartDate && startDate < r.EndDate) ||
                           (endDate > r.StartDate && endDate <= r.EndDate) ||
                           (startDate <= r.StartDate && endDate >= r.EndDate)));

        return !hasConflict;
    }
}
