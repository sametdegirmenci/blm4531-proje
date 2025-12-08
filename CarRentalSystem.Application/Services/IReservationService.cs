using CarRentalSystem.Application.DTOs;

namespace CarRentalSystem.Application.Services;

public interface IReservationService
{
    Task<ReservationDto> CreateReservationAsync(int userId, CreateReservationDto dto);
    Task<ReservationDto> GetReservationByIdAsync(int id);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId);
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<ReservationDto> UpdateReservationAsync(int id, int userId, UpdateReservationDto dto);
    Task CancelReservationAsync(int id, int userId);
    Task<ReservationDto> ConfirmReservationAsync(int id);
    Task<ReservationDto> RejectReservationAsync(int id);
}
