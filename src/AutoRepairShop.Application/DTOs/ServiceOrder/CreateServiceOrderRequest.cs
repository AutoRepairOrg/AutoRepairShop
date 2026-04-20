namespace AutoRepairShop.Application.DTOs.ServiceOrder
{
    public class CreateServiceOrderRequest
    {
        public string Customer { get; set; } //document
        public string Plate { get; set; }
        public Guid Service { get; set; }

        private readonly List<ServiceOrderItemRequest> _items = new();
        public IReadOnlyCollection<ServiceOrderItemRequest> Items => _items;
    }
}
