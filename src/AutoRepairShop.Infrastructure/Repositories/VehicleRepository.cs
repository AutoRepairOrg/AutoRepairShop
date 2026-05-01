using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Data;
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
            _context.Vehicles.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vehicle entity)
        {
            _context.Vehicles.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Vehicle?> GetByPlateAsync(string plate)
        {
            VehiclePlate vehiclePlate = VehiclePlate.Create(plate);
            Vehicle? vehicle = await _context.Vehicles.FirstOrDefaultAsync(c =>
                c.Plate.Value == vehiclePlate.Value
            );
            return vehicle;
        }

        public async Task UpdateAsync(Vehicle entity)
        {
            _context.Vehicles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
