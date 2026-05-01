using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface ISupplyService
        : IBaseService<CreateSupplyRequest, UpdateSupplyRequest, SupplyResponse>
    {
        Task<List<Supply>> GetSuppliesInStockAsync(List<SupplyItemDto> supplyItems);

        Task RestockSuppliesAsync(List<(Guid SupplyId, int Quantity)> supplies);
    }
}
