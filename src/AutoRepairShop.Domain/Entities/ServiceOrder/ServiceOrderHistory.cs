using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Domain.Entities.ServiceOrder
{
    public class ServiceOrderHistory
    {
        public Guid Id { get; private set; }
        public Guid ServiceOrderId { get; private set; }
        public ServiceOrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedById { get; private set; }

        private ServiceOrderHistory() { }

        public ServiceOrderHistory(Guid serviceOrderId, ServiceOrderStatus status, Guid createdById)
        {
            Id = Guid.NewGuid();
            ServiceOrderId = serviceOrderId;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            CreatedById = createdById;
        }
    }
}
