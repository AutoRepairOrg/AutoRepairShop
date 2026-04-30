using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.Models.Supply;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class SupplyRepository(AppDbContext context) : ISupplyRepository
    {
        private readonly AppDbContext _context = context;

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

        public async Task<List<Supply>> GetSuppliesInStockAsync(List<SupplyRequestItem> supplyItems)
        {
            var requestedIds = supplyItems.Select(item => item.SupplyId).ToList();
            var supplies = await _context
                .Supplies.Where(supply => requestedIds.Contains(supply.Id))
                .ToListAsync();

            List<Supply> availableSupplies = [];

            foreach (var supply in supplies)
            {
                var requestedItem = supplyItems.FirstOrDefault(item => item.SupplyId == supply.Id);

                if (requestedItem is null || requestedItem.Quantity > supply.StockQuantity)
                    continue;

                supply.DecreaseStock(requestedItem.Quantity);
                availableSupplies.Add(supply);
            }

            return availableSupplies;
        }

        public async Task UpdateAsync(Supply entity)
        {
            _context.Supplies.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
