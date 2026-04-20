using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Application.DTOs.ServiceOrder
{
    public class ServiceOrderResponse
    {
        public Guid Customer { get; set; }
        public VehicleResponse Vehicle { get; set; }
        public ServiceResponse Service { get; set; }
        public List<ServiceOrderItemResponse> Items { get; set; }
        public ServiceOrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public DateTime? FinishedAt { get; private set; }
    }
}
