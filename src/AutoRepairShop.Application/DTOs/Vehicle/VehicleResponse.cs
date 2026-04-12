namespace AutoRepairShop.Application.DTOs.Vehicle
{
    public class VehicleResponse
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public VehiclePlateDto Plate { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
    }
}
