using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceOrderRepository : IServiceOrderRepository
    {
        public ServiceOrderRepository()
        {
        }

        public Task AddAsync(ServiceOrder serviceOrder)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceOrder?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ServiceOrder serviceOrder)
        {
            throw new NotImplementedException();
        }
    }
}
