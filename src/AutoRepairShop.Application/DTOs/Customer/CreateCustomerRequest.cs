namespace AutoRepairShop.Application.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string Name { get; private set; }
        public DocumentDto Document { get; private set; } // CPF or CNPJ
        public string Phone { get; private set; }
        public string Email { get; private set; }
    }
}
