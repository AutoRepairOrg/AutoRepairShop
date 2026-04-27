using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace AutoRepairShop.Application.Services
{
    public class ServiceOrderService(
        IServiceOrderRepository repository,
        ICustomerService customerService,
        IVehicleService vehicleService,
        IServiceService serviceService,
        ISupplyService supplyService
    ) : IServiceOrderService
    {
        private readonly IServiceOrderRepository _repository = repository;
        private readonly ICustomerService _customerService = customerService;
        private readonly IVehicleService _vehicleService = vehicleService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ISupplyService _supplyService = supplyService;

        public async Task CreateServiceOrderAsync(CreateServiceOrderRequest request)
        {
            // ● Identificação do cliente por CPF/CNPJ;
            Customer customer = await _customerService.GetByCpfCnpjAsync(request.CustomerDocument);

            // ● Cadastro de veículo (placa, marca, modelo, ano);
            Vehicle vehicle = await _vehicleService.GetOrCreateAsync(request.Vehicle, customer.Id);

            // ● Inclusão dos serviços solicitados (exemplo: troca de óleo, alinhamento);
            List<Service> services = await _serviceService.GetServicesByIdsAsync(
                request.ServiceIds
            );

            // ● Possibilidade de incluir peças e insumos necessários;
            List<Supply> supplies = await _supplyService.GetSuppliesInStockAsync(
                request.SupplyItems
            );

            if (services.IsNullOrEmpty() && supplies.IsNullOrEmpty())
                throw new DomainException(
                    "No services or supplies available for the service order."
                );

            // ● Orçamento gerado automaticamente com base nos serviços e peças;
            var serviceOrder = new ServiceOrder
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                VehicleId = vehicle.Id,
            };
            services.ForEach(s => serviceOrder.AddService(s.Id));
            supplies.ForEach(s =>
                serviceOrder.AddSupply(
                    s.Id,
                    request.SupplyItems.First(i => i.SupplyId == s.Id).Quantity
                )
            );

            await _repository.AddAsync(serviceOrder);

            // ● Envio do orçamento ao cliente para aprovação.
            SendOrderForApproval(serviceOrder); // Como passado no grupo do Discord, não é necessário implementar o envio real, apenas simular a ação.
        }

        private static void SendOrderForApproval(ServiceOrder serviceOrder)
        {
            Console.WriteLine(
                $"Orçamento para serviço #{serviceOrder.Id} enviado para aprovação do cliente #{serviceOrder.CustomerId}."
            );
        }

        public async Task StartDiagnosisAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.StartDiagnosis();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task RequestApprovalAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.RequestApproval();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task ApproveAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Approve();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task FinishAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Finish();

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task DeliverAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            serviceOrder.Deliver();

            await _repository.UpdateAsync(serviceOrder);
        }
    }
}
