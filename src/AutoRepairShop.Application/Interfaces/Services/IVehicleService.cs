using AutoRepairShop.Application.DTOs.Vehicle;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface IVehicleService
        : IBaseService<CreateVehicleRequest, UpdateVehicleRequest, VehicleResponse>
    {
        Task<Vehicle> GetOrCreateAsync(VehicleDto request, Guid customerId);
    }
}
