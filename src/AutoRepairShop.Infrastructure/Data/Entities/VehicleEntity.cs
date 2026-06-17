namespace AutoRepairShop.Infrastructure.Data.Entities;

public class VehicleEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}
