using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Application.Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly IServiceOrderRepository _repository;

        public ServiceOrderService(
            IServiceOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateAsync(
            Guid customerId,
            Guid vehicleId,
            Guid serviceId)
        {
            var serviceOrder = new ServiceOrder(
                customerId,
                vehicleId,
                serviceId);

            await _repository.AddAsync(serviceOrder);

            return serviceOrder.Id;
        }

        public async Task AddSupplyAsync(
            Guid serviceOrderId,
            Guid supplyId,
            string supplyName,
            int quantity,
            decimal unitPrice,
            decimal servicePrice)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.AddSupply(
                supplyId,
                supplyName,
                quantity,
                unitPrice,
                servicePrice);

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task StartDiagnosisAsync(Guid serviceOrderId)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.StartDiagnosis();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task RequestApprovalAsync(Guid serviceOrderId)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.RequestApproval();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task ApproveAsync(Guid serviceOrderId)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Approve();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task FinishAsync(Guid serviceOrderId)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Finish();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task DeliverAsync(Guid serviceOrderId)
        {
            var serviceOrder = await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Deliver();

            await _repository.UpdateAsync(serviceOrder);
        }
    }
}
