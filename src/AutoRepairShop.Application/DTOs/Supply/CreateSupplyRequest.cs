namespace AutoRepairShop.Application.DTOs.Supply
{
    public class CreateSupplyRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
