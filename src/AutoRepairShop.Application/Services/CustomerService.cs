using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.Interfaces;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Application.Services
{
    public class CustomerService(
        ICustomerRepository repository,
        IMapper mapper,
        IPasswordHasher passwordHasher
    ) : ICustomerService
    {
        private readonly ICustomerRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            var hash = _passwordHasher.Hash(request.Password);

            var customer = new Customer(
                request.Name,
                Document.Create(request.Document),
                request.Phone,
                request.Username,
                hash
            );

            await _repository.AddAsync(customer);

            return _mapper.Map<CustomerResponse>(customer);
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new Exception("Customer not found.");

            await _repository.DeleteAsync(result);
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerResponse>>(result);
        }

        public async Task<Customer> GetByCpfCnpjAsync(string cpfCnpj)
        {
            Customer customer =
                await _repository.GetByCpfCnpjAsync(cpfCnpj)
                ?? throw new Exception("Customer not found.");
            return customer;
        }

        public async Task<CustomerResponse> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<CustomerResponse>(result);
        }

        public async Task<CustomerResponse> UpdateAsync(UpdateCustomerRequest request)
        {
            var result = await _repository.GetByIdAsync(request.Id);

            if (result == null)
                throw new Exception("Customer not found.");

            //TODO: Criação de exceções específicas/customizadas

            var hash = _passwordHasher.Hash(request.Password);

            result.Update(request.Name, request.Phone, request.Username, hash);

            await _repository.UpdateAsync(result);

            return _mapper.Map<CustomerResponse>(result);
        }
    }
}
