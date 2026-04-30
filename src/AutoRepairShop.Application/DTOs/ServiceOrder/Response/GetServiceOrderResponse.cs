using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Application.DTOs.ServiceOrder.Response
{
    public class GetServiceOrderResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public ServiceOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public List<ServiceOrderItemResponse> Services { get; set; } = [];
        public List<ServiceOrderSupplyItemResponse> Supplies { get; set; } = [];
        public List<ServiceOrderHistoryResponse> History { get; set; } = [];
    }

    public class ServiceOrderItemResponse
    {
        public Guid ServiceId { get; set; }
    }

    public class ServiceOrderSupplyItemResponse
    {
        public Guid SupplyId { get; set; }
        public int Quantity { get; set; }
    }

    public class ServiceOrderHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid ServiceOrderId { get; set; }
        public ServiceOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
    }
}
