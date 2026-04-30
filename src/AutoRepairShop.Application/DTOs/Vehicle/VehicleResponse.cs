namespace AutoRepairShop.Application.DTOs.Vehicle
{
    public class VehicleResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public VehiclePlateDto Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
}
