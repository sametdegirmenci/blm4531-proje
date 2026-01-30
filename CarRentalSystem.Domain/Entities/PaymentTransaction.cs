using System;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class PaymentTransaction
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionReference { get; set; } = string.Empty;

    // Navigation properties
    public virtual Reservation Reservation { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
