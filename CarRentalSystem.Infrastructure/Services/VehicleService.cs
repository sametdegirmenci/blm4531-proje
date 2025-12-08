using AutoMapper;
using CarRentalSystem.Application.DTOs;
using CarRentalSystem.Application.Services;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Exceptions;
using CarRentalSystem.Domain.Interfaces;

namespace CarRentalSystem.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync();
        return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    public async Task<VehicleDto> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            throw new NotFoundException(nameof(Vehicle), id);
        }

        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var vehicle = _mapper.Map<Vehicle>(dto);
        vehicle.CreatedAt = DateTime.UtcNow;
        vehicle.IsAvailable = true;

        vehicle = await _vehicleRepository.AddAsync(vehicle);

        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            throw new NotFoundException(nameof(Vehicle), id);
        }

        // Update properties
        vehicle.Brand = dto.Brand;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.PricePerDay = dto.PricePerDay;
        vehicle.IsAvailable = dto.IsAvailable;
        vehicle.ImageUrl = dto.ImageUrl;
        vehicle.LicensePlate = dto.LicensePlate;

        await _vehicleRepository.UpdateAsync(vehicle);

        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task DeleteVehicleAsync(int id)
    {
        if (!await _vehicleRepository.ExistsAsync(id))
        {
            throw new NotFoundException(nameof(Vehicle), id);
        }

        await _vehicleRepository.DeleteAsync(id);
    }
}
