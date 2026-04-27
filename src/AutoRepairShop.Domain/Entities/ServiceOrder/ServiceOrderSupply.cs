namespace AutoRepairShop.Domain.Entities.ServiceOrder
{
    public class ServiceOrderSupply
    {
        public Guid ServiceOrderId { get; private set; }
        public Guid SupplyId { get; private set; }
        public int Quantity { get; private set; }

        private ServiceOrderSupply() { }

        public ServiceOrderSupply(Guid serviceOrderId, Guid supplyId, int quantity)
        {
            ServiceOrderId = serviceOrderId;
            SupplyId = supplyId;
            Quantity = quantity;
        }
    }
}
