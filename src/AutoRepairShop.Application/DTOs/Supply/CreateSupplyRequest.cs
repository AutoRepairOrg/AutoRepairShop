namespace AutoRepairShop.Application.DTOs.Supply
{
    public class CreateSupplyRequest
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
    }
}
