using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Exceptions;

namespace AutoRepairShop.Domain.Entities.ServiceOrder
{
    public class ServiceOrder()
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Received;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        private readonly List<ServiceOrderService> _services = [];
        public IReadOnlyCollection<ServiceOrderService> Services => _services;
        private readonly List<ServiceOrderSupply> _supplies = [];
        public IReadOnlyCollection<ServiceOrderSupply> Supplies => _supplies;

        public void AddService(Guid serviceId)
        {
            _services.Add(new ServiceOrderService(Id, serviceId));
        }

        public void AddSupply(Guid supplyId, int quantity)
        {
            _supplies.Add(new ServiceOrderSupply(Id, supplyId, quantity));
        }

        public void StartDiagnosis()
        {
            if (Status != ServiceOrderStatus.Received)
                throw new DomainException("Service order must be received.");

            Status = ServiceOrderStatus.InDiagnosis;
        }

        public void RequestApproval()
        {
            if (Status != ServiceOrderStatus.InDiagnosis)
                throw new DomainException("Service order must be in diagnosis.");

            Status = ServiceOrderStatus.WaitingApproval;
        }

        public void Approve()
        {
            if (Status != ServiceOrderStatus.WaitingApproval)
                throw new DomainException("Service order must be awaiting approval.");

            Status = ServiceOrderStatus.InExecution;
            StartedAt = DateTime.UtcNow;
        }

        public void Finish()
        {
            if (Status != ServiceOrderStatus.InExecution)
                throw new DomainException("Service order must be in execution.");

            Status = ServiceOrderStatus.Finished;
            FinishedAt = DateTime.UtcNow;
        }

        public void Deliver()
        {
            if (Status != ServiceOrderStatus.Finished)
                throw new DomainException("Service order must be finished.");

            Status = ServiceOrderStatus.Delivered;
        }
    }
}
