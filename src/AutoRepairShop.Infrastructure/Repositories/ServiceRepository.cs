using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceRepository(AppDbContext context) : IServiceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(Service entity)
        {
            _context.Services.Add(entity.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Service entity)
        {
            var persisted = await _context.Services.FirstOrDefaultAsync(s => s.Id == entity.Id);

            if (persisted is null)
            {
                return;
            }

            _context.Services.Remove(persisted);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return [.. (await _context.Services.AsNoTracking().ToListAsync()).Select(x => x.ToDomain())];
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Services.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return entity?.ToDomain();
        }

        public async Task<List<Service>> GetServicesByIdsAsync(List<Guid> serviceIds)
        {
            var entities = await _context
                .Services.AsNoTracking()
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();

            return [.. entities.Select(x => x.ToDomain())];
        }

        public async Task UpdateAsync(Service entity)
        {
            var persisted = await _context.Services.FirstOrDefaultAsync(s => s.Id == entity.Id);

            if (persisted is null)
            {
                return;
            }

            persisted.Name = entity.Name;
            persisted.Description = entity.Description;
            persisted.Price = entity.Price;

            await _context.SaveChangesAsync();
        }
    }
}
