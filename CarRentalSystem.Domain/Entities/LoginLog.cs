using System;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class LoginLog
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public DateTime AttemptDate { get; set; }
    public bool IsSuccess { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? FailureReason { get; set; }

    // Navigation property
    public virtual User? User { get; set; }
}
