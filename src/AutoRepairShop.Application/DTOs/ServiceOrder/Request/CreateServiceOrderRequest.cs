using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.DTOs.Vehicle;

namespace AutoRepairShop.Application.DTOs.ServiceOrder.Request
{
    public class CreateServiceOrderRequest
    {
        public required string CustomerDocument { get; set; }
        public required VehicleDto Vehicle { get; set; }
        public List<Guid> ServiceIds { get; set; } = [];
        public List<SupplyItemDto> SupplyItems { get; set; } = [];
    }
}
