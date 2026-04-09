using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Enums;

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

    public void AddSupply(Guid supplyId, int quantity, decimal unitPrice)
    {
        //if (_items.Any(i => i.SupplyId == supplyId))
        //    throw new DomainException("Supply já adicionado à ordem.");

        _items.Add(new ServiceOrderItem(this.Id, supplyId, quantity, unitPrice));
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Subtotal);
    }
}