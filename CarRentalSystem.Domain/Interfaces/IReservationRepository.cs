using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);
    Task<IEnumerable<Reservation>> GetReservationsByVehicleIdAsync(int vehicleId);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
    Task<bool> HasConflictingReservationAsync(int vehicleId, DateTime startDate, DateTime endDate, int? excludeReservationId = null);
}
