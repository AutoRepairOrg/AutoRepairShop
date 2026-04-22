using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceOrderRepository(AppDbContext context) : IServiceOrderRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(ServiceOrder serviceOrder)
        {
            await _context.ServiceOrders.AddAsync(serviceOrder);
        }

        public async Task<ServiceOrder?> GetByIdAsync(Guid id)
        {
            return await _context
                .ServiceOrders.Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(ServiceOrder serviceOrder)
        {
            _context.ServiceOrders.Update(serviceOrder);
            await _context.SaveChangesAsync();
        }
    }
}
