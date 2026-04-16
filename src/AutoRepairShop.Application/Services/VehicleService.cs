using AutoMapper;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;
        private readonly IMapper _mapper;

        public VehicleService(
            IVehicleRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request)
        {
            //TODO: Validar se o cliente existe

           var vehicle = new Vehicle(
               request.CustomerId,
               VehiclePlate.Create(request.Plate.Value),
               request.Brand,
               request.Model,
               request.Year);

            await _repository.AddAsync(vehicle);

            return _mapper.Map<VehicleResponse>(vehicle);
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new Exception("Vehicle not found.");

            await _repository.DeleteAsync(result);
        }

        public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<VehicleResponse>>(result);
        }

        public async Task<VehicleResponse> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<VehicleResponse>(result);
        }

        public async Task<VehicleResponse> UpdateAsync(UpdateVehicleRequest request)
        {
            var result = await _repository.GetByIdAsync(request.Id);

            if (result == null)
                throw new Exception("Vehicle not found.");

            //TODO: Criação de exceções específicas/customizadas

            result.Update(
                request.Brand,
                request.Model,
                request.Year);

            await _repository.UpdateAsync(result);

            return _mapper.Map<VehicleResponse>(result);
        }
    }
}
