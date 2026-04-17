using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class SupplyRepository : ISupplyRepository
    {
        private readonly AppDbContext _context;

        public SupplyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Supply entity)
        {
            _context.Supplies.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Supply entity)
        {
            _context.Supplies.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Supply>> GetAllAsync()
        {
            return await _context.Supplies.ToListAsync();
        }

        public async Task<Supply?> GetByIdAsync(Guid id)
        {
            return await _context.Supplies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Supply entity)
        {
            _context.Supplies.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
