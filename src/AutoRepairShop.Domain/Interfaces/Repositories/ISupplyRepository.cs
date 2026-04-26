using AutoRepairShop.Domain.Models.Supply;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface ISupplyRepository : IRepository<Supply>
    {
        Task<List<Supply>> GetSuppliesInStockAsync(List<SupplyRequestItem> supplyItems);
    }
}
