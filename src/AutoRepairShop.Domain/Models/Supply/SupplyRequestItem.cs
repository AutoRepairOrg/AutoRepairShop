namespace AutoRepairShop.Domain.Models.Supply
{
    public class SupplyRequestItem(Guid supplyId, int quantity)
    {
        public Guid SupplyId { get; } = supplyId;
        public int Quantity { get; } = quantity;
    }
}
