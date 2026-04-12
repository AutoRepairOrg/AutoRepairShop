namespace AutoRepairShop.Application.DTOs.Vehicle
{
    public class CreateVehicleRequest
    {
        public Guid CustomerId { get; private set; }
        public VehiclePlateDto Plate { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
    }
}
