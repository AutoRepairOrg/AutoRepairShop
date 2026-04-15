using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByUserNameAsync(string username);
    }
}
