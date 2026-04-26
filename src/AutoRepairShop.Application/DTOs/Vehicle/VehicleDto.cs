namespace AutoRepairShop.Application.DTOs.Vehicle
{
    public class VehicleDto
    {
        public required string Plate { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required int Year { get; set; }
    }
}
