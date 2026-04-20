namespace AutoRepairShop.Application.DTOs.ServiceOrder
{
    public class ServiceOrderItemRequest
    {
        public Guid SupplyId { get; }
        public int Quantity { get; }
    }
}
