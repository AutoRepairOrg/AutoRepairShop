using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Admin entity)
        {
            _context.Admins.Add(entity.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Admin entity)
        {
            var persisted = await _context.Admins.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            _context.Admins.Remove(persisted);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Admin>> GetAllAsync()
        {
            return [.. (await _context.Admins.ToListAsync()).Select(x => x.ToDomain())];
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Admins.FirstOrDefaultAsync(c => c.Id == id);
            return entity?.ToDomain();
        }

        public async Task<Admin?> GetByUserNameAsync(string username)
        {
            var entity = await _context.Admins.FirstOrDefaultAsync(c => c.Username == username);
            return entity?.ToDomain();
        }

        public async Task UpdateAsync(Admin entity)
        {
            var persisted = await _context.Admins.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            persisted.Name = entity.Name;
            persisted.Department = entity.Department;
            persisted.Username = entity.Username;
            persisted.Password = entity.Password;

            await _context.SaveChangesAsync();
        }
    }
}
