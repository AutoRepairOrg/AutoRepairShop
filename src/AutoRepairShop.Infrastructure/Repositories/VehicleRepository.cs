using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;

        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vehicle entity)
        {
            _context.Vehicles.Add(entity.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vehicle entity)
        {
            var persisted = await _context.Vehicles.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            _context.Vehicles.Remove(persisted);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vehicle>> GetAllAsync()
        {
            return [.. (await _context.Vehicles.ToListAsync()).Select(x => x.ToDomain())];
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Vehicles.FirstOrDefaultAsync(c => c.Id == id);
            return entity?.ToDomain();
        }

        public async Task<Vehicle?> GetByPlateAsync(string plate)
        {
            var entity = await _context.Vehicles.FirstOrDefaultAsync(c => c.Plate == plate);
            return entity?.ToDomain();
        }

        public async Task UpdateAsync(Vehicle entity)
        {
            var persisted = await _context.Vehicles.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            persisted.Plate = entity.Plate.Value;
            persisted.Brand = entity.Brand;
            persisted.Model = entity.Model;
            persisted.Year = entity.Year;

            await _context.SaveChangesAsync();
        }
    }
}
