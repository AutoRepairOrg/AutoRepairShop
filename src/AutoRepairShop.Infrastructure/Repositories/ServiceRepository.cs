using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceRepository(AppDbContext context) : IServiceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(Service entity)
        {
            _context.Services.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Service entity)
        {
            _context.Services.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _context.Services.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Service>> GetServicesByIdsAsync(List<Guid> serviceIds)
        {
            return await _context.Services.Where(s => serviceIds.Contains(s.Id)).ToListAsync();
        }

        public async Task UpdateAsync(Service entity)
        {
            _context.Services.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
