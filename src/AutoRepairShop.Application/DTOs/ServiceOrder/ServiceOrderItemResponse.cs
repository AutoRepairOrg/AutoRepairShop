namespace AutoRepairShop.Application.DTOs.ServiceOrder
{
    public class ServiceOrderItemResponse
    {
        public Guid SupplyId { get; }
        public string SupplyName { get; }
        public decimal UnitPrice { get; }
        public int Quantity { get; }

        public decimal Subtotal => UnitPrice * Quantity;
    }
}
