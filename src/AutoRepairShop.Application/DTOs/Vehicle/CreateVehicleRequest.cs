namespace AutoRepairShop.Application.DTOs.Vehicle
{
    public class CreateVehicleRequest
    {
        public Guid CustomerId { get; set; }
        public VehiclePlateDto Plate { get; set; } = null!;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
