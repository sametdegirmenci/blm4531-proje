using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class Reservation
{
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Vehicle Vehicle { get; set; } = null!;

    // Business logic
    public int GetRentalDays() => (EndDate - StartDate).Days;
    public bool IsActive() => Status == ReservationStatus.Pending || Status == ReservationStatus.Confirmed;
}
