using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<Vehicle?> GetByPlateAsync(string plate);
    }
}
 