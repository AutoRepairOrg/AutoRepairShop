namespace AutoRepairShop.Application.DTOs.ServiceOrder
{
    public class ServiceOrderSummary
    {
        public Guid ServiceOrderId { get; set; }
        public List<ServiceSummary> Services { get; set; } = [];
        public List<SupplySummary> Supplies { get; set; } = [];
        public decimal TotalAmount { get; set; }
    }

    public class ServiceSummary
    {
        public Guid ServiceId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class SupplySummary
    {
        public Guid SupplyId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
