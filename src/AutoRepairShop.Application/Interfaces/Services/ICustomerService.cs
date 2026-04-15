using AutoRepairShop.Application.DTOs.Customer;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface ICustomerService : IBaseService<CreateCustomerRequest, UpdateCustomerRequest, CustomerResponse>
    {
    }
}