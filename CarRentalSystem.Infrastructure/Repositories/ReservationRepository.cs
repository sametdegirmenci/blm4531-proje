using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Domain.Interfaces;
using CarRentalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation> GetByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Reservation> AddAsync(Reservation entity)
    {
        await _context.Reservations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Reservation entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Reservations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Reservations.AnyAsync(r => r.ReservationId == id);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Include(r => r.Vehicle)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByVehicleIdAsync(int vehicleId)
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == ReservationStatus.Pending ||
                       r.Status == ReservationStatus.Confirmed)
            .OrderBy(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<bool> HasConflictingReservationAsync(int vehicleId, DateTime startDate, DateTime endDate, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.VehicleId == vehicleId &&
                       (r.Status == ReservationStatus.Pending ||
                        r.Status == ReservationStatus.Confirmed) &&
                       ((startDate >= r.StartDate && startDate < r.EndDate) ||
                        (endDate > r.StartDate && endDate <= r.EndDate) ||
                        (startDate <= r.StartDate && endDate >= r.EndDate)));

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.ReservationId != excludeReservationId.Value);
        }

        return await query.AnyAsync();
    }
}
