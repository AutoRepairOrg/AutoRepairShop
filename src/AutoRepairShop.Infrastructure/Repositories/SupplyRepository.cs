using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.Models.Supply;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class SupplyRepository(AppDbContext context) : ISupplyRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(Supply entity)
        {
            _context.Supplies.Add(entity.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Supply entity)
        {
            var persisted = await _context.Supplies.FirstOrDefaultAsync(s => s.Id == entity.Id);

            if (persisted is null)
            {
                return;
            }

            _context.Supplies.Remove(persisted);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Supply>> GetAllAsync()
        {
            return [.. (await _context.Supplies.AsNoTracking().ToListAsync()).Select(x => x.ToDomain())];
        }

        public async Task<Supply?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Supplies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return entity?.ToDomain();
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

                if (requestedItem is null)
                    continue;

                var domainSupply = supply.ToDomain();

                try
                {
                    domainSupply.DecreaseStock(requestedItem.Quantity);
                }
                catch (DomainException)
                {
                    continue;
                }

                supply.StockQuantity = domainSupply.StockQuantity;
                availableSupplies.Add(domainSupply);
            }

            return availableSupplies;
        }

        public async Task UpdateAsync(Supply entity)
        {
            var persisted = await _context.Supplies.FirstOrDefaultAsync(s => s.Id == entity.Id);

            if (persisted is null)
            {
                return;
            }

            persisted.Name = entity.Name;
            persisted.Price = entity.Price;
            persisted.StockQuantity = entity.StockQuantity;

            await _context.SaveChangesAsync();
        }
    }
}
