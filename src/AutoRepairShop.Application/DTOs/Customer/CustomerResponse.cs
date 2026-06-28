namespace AutoRepairShop.Application.DTOs.Customer
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Teste { get; set; } = "ambiente atualizado em tempo real com pull request";
    }
}
