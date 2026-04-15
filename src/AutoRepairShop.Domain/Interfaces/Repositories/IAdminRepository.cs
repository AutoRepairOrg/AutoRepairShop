using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByUserNameAsync(string username);
    }
}
