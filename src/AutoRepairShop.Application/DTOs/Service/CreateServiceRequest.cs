namespace AutoRepairShop.Application.DTOs.Service
{
    public class CreateServiceRequest
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
    }
}
