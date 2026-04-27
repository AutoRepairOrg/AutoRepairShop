namespace AutoRepairShop.Domain.Entities.ServiceOrder
{
    public class ServiceOrderService
    {
        public Guid ServiceOrderId { get; private set; }
        public Guid ServiceId { get; private set; }

        private ServiceOrderService() { }

        public ServiceOrderService(Guid serviceOrderId, Guid serviceId)
        {
            ServiceOrderId = serviceOrderId;
            ServiceId = serviceId;
        }
    }
}
