namespace AutoRepairShop.Application.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public string Document { get; set; } // CPF or CNPJ
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
