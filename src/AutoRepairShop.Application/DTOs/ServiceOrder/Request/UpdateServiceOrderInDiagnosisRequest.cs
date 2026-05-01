using AutoRepairShop.Application.DTOs.Supply;

namespace AutoRepairShop.Application.DTOs.ServiceOrder.Request
{
    public class UpdateServiceOrderInDiagnosisRequest
    {
        public List<Guid> ServiceIds { get; set; } = [];
        public List<SupplyItemDto> SupplyItems { get; set; } = [];
    }
}
