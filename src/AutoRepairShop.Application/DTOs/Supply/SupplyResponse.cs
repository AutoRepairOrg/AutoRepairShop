namespace AutoRepairShop.Application.DTOs.Supply
{
    public class SupplyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
