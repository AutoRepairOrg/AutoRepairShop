namespace AutoRepairShop.Infrastructure.Data.Entities;

public class SupplyEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
