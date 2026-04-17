namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface IServiceOrderRepository
    {
        Task AddAsync(ServiceOrder serviceOrder);
        Task<ServiceOrder?> GetByIdAsync(Guid id);
        Task UpdateAsync(ServiceOrder serviceOrder);
    }
}
