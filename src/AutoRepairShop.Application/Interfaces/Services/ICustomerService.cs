using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface ICustomerService
        : IBaseService<CreateCustomerRequest, UpdateCustomerRequest, CustomerResponse>
    {
        Task<Customer> GetByCpfCnpjAsync(string cpfCnpj);

        Task<Customer> GetEntityByIdAsync(Guid id);
    }
}
