using AutoMapper;
using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Application.Services
{
    public class ServiceService(IServiceRepository repository, IMapper mapper) : IServiceService
    {
        private readonly IServiceRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<ServiceResponse> CreateAsync(CreateServiceRequest request)
        {
            var service = new Service(request.Name, request.Description, request.Price);

            await _repository.AddAsync(service);

            return _mapper.Map<ServiceResponse>(service);
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new Exception("Service not found.");

            await _repository.DeleteAsync(result);
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceResponse>>(result);
        }

        public async Task<ServiceResponse> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<ServiceResponse>(result);
        }

        public async Task<List<Service>> GetServicesByIdsAsync(List<Guid> serviceIds)
        {
            List<Service> services = await _repository.GetServicesByIdsAsync(serviceIds);
            return services;
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateServiceRequest request)
        {
            var result = await _repository.GetByIdAsync(request.Id);

            if (result == null)
                throw new Exception("Service not found.");

            //TODO: Criação de exceções específicas/customizadas

            result.Update(request.Name, request.Description, request.Price);

            await _repository.UpdateAsync(result);

            return _mapper.Map<ServiceResponse>(result);
        }
    }
}
