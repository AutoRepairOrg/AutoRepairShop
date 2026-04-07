using AutoRepairShop.Domain.Enums;

public class ServiceOrder
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid VehicleId { get; private set; }

    public ServiceOrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    protected ServiceOrder() { }

    public ServiceOrder(Guid customerId, Guid vehicleId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        VehicleId = vehicleId;
        Status = ServiceOrderStatus.Received;
        CreatedAt = DateTime.UtcNow;
        TotalAmount = 0;
    }

    public void SetTotalAmount(decimal amount)
    {
        TotalAmount = amount;
    }

    public void ChangeStatus(ServiceOrderStatus status)
    {
        Status = status;
    }
}