namespace AutoRepairShop.Application.DTOs.Supply
{
    public class UpdateSupplyRequest
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
    }
}
