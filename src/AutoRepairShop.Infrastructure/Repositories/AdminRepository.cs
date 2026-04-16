using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        //TODO: Implementação de CRUD de admin
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(Admin entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Admin entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Admin>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Admin?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Admin?> GetByUserNameAsync(string username)
        {
            return await _context.Admins.FirstOrDefaultAsync(c => c.Username == username);
        }

        public Task UpdateAsync(Admin entity)
        {
            throw new NotImplementedException();
        }
    }
}
