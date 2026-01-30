using CarRentalSystem.Application.DTOs;

namespace CarRentalSystem.Application.Services;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
    Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync();
    Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate);
    Task<VehicleDto> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);
    Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
    Task DeleteVehicleAsync(int id);
}
