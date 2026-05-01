using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<List<Service>> GetServicesByIdsAsync(List<Guid> serviceIds);
    }
}
