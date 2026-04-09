namespace AutoRepairShop.Domain.Entities
{
    public class ServiceOrderItem
    {
        public Guid Id { get; private set; }
        public Guid ServiceOrderId { get; private set; }
        public Guid SupplyId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; private set; }
        public decimal Subtotal => Quantity * UnitPrice;

        protected ServiceOrderItem() { }

        internal ServiceOrderItem(Guid serviceOrderId, Guid supplyId, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            ServiceOrderId = serviceOrderId;
            SupplyId = supplyId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
