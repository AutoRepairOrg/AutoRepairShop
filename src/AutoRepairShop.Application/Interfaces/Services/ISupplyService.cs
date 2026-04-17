using AutoRepairShop.Application.DTOs.Supply;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface ISupplyService : IBaseService<CreateSupplyRequest, UpdateSupplyRequest, SupplyResponse>
    {
    }
}
