using System;

namespace CarRentalSystem.Application.DTOs;

public record LoginLogDto(
    int Id,
    string Email,
    int? UserId,
    DateTime AttemptDate,
    bool IsSuccess,
    string? IpAddress,
    string? UserAgent,
    string? FailureReason
);
