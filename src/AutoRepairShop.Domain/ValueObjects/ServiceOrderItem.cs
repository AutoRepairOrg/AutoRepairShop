namespace AutoRepairShop.Domain.ValueObjects
{
    public class ServiceOrderItem
    {
        public Guid SupplyId { get; }
        public string SupplyName { get; } = string.Empty;
        public decimal UnitPrice { get; }
        public int Quantity { get; }

        public decimal Subtotal => UnitPrice * Quantity;

        private ServiceOrderItem() { }

        public ServiceOrderItem(Guid supplyId, string supplyName, decimal unitPrice, int quantity)
        {
            if (supplyId == Guid.Empty)
                throw new ArgumentException("SupplyId is required.");

            if (string.IsNullOrWhiteSpace(supplyName))
                throw new ArgumentException("Supply name is required.");

            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            SupplyId = supplyId;
            SupplyName = supplyName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
