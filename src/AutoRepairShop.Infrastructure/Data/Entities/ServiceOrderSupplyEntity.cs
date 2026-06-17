namespace AutoRepairShop.Infrastructure.Data.Entities;

public class ServiceOrderSupplyEntity
{
    public Guid ServiceOrderId { get; set; }
    public Guid SupplyId { get; set; }
    public int Quantity { get; set; }
}
