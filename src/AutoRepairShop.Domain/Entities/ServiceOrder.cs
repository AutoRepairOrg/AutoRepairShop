using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.ValueObjects;

public class ServiceOrder
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }

    public Guid ServiceId { get; private set; }

    private readonly List<ServiceOrderItem> _items = new();
    public IReadOnlyCollection<ServiceOrderItem> Items => _items;

    public ServiceOrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }

    protected ServiceOrder() { }

    public ServiceOrder(Guid customerId, Guid vehicleId, Guid serviceId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        VehicleId = vehicleId;
        ServiceId = serviceId;
        Status = ServiceOrderStatus.Received;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddSupply(Guid supplyId,string supplyName, int quantity, decimal unitPrice, decimal servicePrice)
    {
        if (Status != ServiceOrderStatus.Received &&
            Status != ServiceOrderStatus.InDiagnosis)
            throw new DomainException("Cannot add supplies at this stage.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        var supply = new ServiceOrderItem(supplyId, supplyName, unitPrice, quantity);
        _items.Add(supply);

        RecalculateTotal(servicePrice);
    }

    private void RecalculateTotal(decimal servicePrice)
    {
        TotalAmount = servicePrice + _items.Sum(s => s.Subtotal);
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