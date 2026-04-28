using AutoMapper;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.Models.Supply;

namespace AutoRepairShop.Application.Services
{
    public class SupplyService(ISupplyRepository repository, IMapper mapper) : ISupplyService
    {
        private readonly ISupplyRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SupplyResponse> CreateAsync(CreateSupplyRequest request)
        {
            var supply = new Supply(request.Name, request.Price, request.StockQuantity);

            await _repository.AddAsync(supply);
            return _mapper.Map<SupplyResponse>(supply);
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new Exception("Supply not found.");

            await _repository.DeleteAsync(result);
        }

        public async Task<IEnumerable<SupplyResponse>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplyResponse>>(result);
        }

        public async Task<SupplyResponse> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<SupplyResponse>(result);
        }

        public async Task<List<Supply>> GetSuppliesInStockAsync(List<SupplyItemDto> supplyItems)
        {
            List<SupplyRequestItem> supplyRequestItems = _mapper.Map<List<SupplyRequestItem>>(
                supplyItems
            );
            List<Supply> supplies = await _repository.GetSuppliesInStockAsync(supplyRequestItems);
            return supplies;
        }

        public async Task<SupplyResponse> UpdateAsync(UpdateSupplyRequest request)
        {
            var result = await _repository.GetByIdAsync(request.Id);

            if (result == null)
                throw new Exception("Supply not found.");

            //TODO: Criação de exceções específicas/customizadas

            result.Update(request.Name, request.Price, request.StockQuantity);

            await _repository.UpdateAsync(result);

            return _mapper.Map<SupplyResponse>(result);
        }

        public async Task RestockSuppliesAsync(List<(Guid SupplyId, int Quantity)> supplies)
        {
            foreach (var (supplyId, quantity) in supplies)
            {
                var supply =
                    await _repository.GetByIdAsync(supplyId)
                    ?? throw new Exception($"Supply with ID {supplyId} not found.");
                supply.IncreaseStock(quantity);

                await _repository.UpdateAsync(supply);
            }
        }
    }
}
