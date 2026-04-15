using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            var customer = new Customer(
                request.Name,
                Document.Create(request.Document),
                request.Phone,
                request.Username,
                request.Password
            );

            await _repository.AddAsync(customer);

            return _mapper.Map<CustomerResponse>(customer);
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerResponse>>(result);
        }

        public Task<CustomerResponse> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerResponse> UpdateAsync(UpdateCustomerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
