using AutoMapper;
using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.DTOs.ServiceOrder.Response;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
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
        ISupplyService supplyService,
        IMapper mapper
    ) : IServiceOrderService
    {
        private readonly IServiceOrderRepository _repository = repository;
        private readonly ICustomerService _customerService = customerService;
        private readonly IVehicleService _vehicleService = vehicleService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ISupplyService _supplyService = supplyService;
        private readonly IMapper _mapper = mapper;

        public async Task CreateServiceOrderAsync(
            CreateServiceOrderRequest request,
            Guid createdById
        )
        {
            Customer customer = await _customerService.GetByCpfCnpjAsync(request.CustomerDocument);

            Vehicle vehicle = await _vehicleService.GetOrCreateAsync(request.Vehicle, customer.Id);

            List<Service> services = await _serviceService.GetServicesByIdsAsync(
                request.ServiceIds
            );

            List<Supply> supplies = await _supplyService.GetSuppliesInStockAsync(
                request.SupplyItems
            );

            if (services.IsNullOrEmpty() && supplies.IsNullOrEmpty())
                throw new DomainException(
                    "No services or supplies available for the service order."
                );

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

            serviceOrder.AddHistory(serviceOrder.Status, createdById);

            await _repository.AddAsync(serviceOrder);
        }

        public async Task<GetServiceOrderResponse> GetByIdAsync(Guid serviceOrderId)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            return _mapper.Map<GetServiceOrderResponse>(serviceOrder);
        }

        public async Task<IEnumerable<GetServiceOrderResponse>> GetAllAsync(
            ServiceOrderStatus? status
        )
        {
            var serviceOrders = await _repository.GetAllAsync(status);
            return _mapper.Map<IEnumerable<GetServiceOrderResponse>>(serviceOrders);
        }

        public async Task AdvanceStatusAsync(Guid serviceOrderId, Guid changedById)
        {
            var serviceOrder =
                await _repository.GetByIdAsync(serviceOrderId)
                ?? throw new DomainException("Service order not found.");

            switch (serviceOrder.Status)
            {
                case ServiceOrderStatus.Received:
                    serviceOrder.StartDiagnosis();
                    break;
                case ServiceOrderStatus.InDiagnosis:
                    serviceOrder.RequestApproval();
                    break;
                case ServiceOrderStatus.InExecution:
                    serviceOrder.Finish();
                    break;
                case ServiceOrderStatus.Finished:
                    serviceOrder.Deliver();
                    break;
                case ServiceOrderStatus.WaitingApproval:
                    throw new DomainException(
                        "Only customer can decide while status is waiting approval."
                    );
                case ServiceOrderStatus.Canceled:
                    throw new DomainException("Canceled service order cannot be advanced.");
                case ServiceOrderStatus.Delivered:
                    throw new DomainException("Service order is already delivered.");
                default:
                    throw new DomainException("Invalid service order status transition.");
            }

            serviceOrder.AddHistory(serviceOrder.Status, changedById);

            await _repository.UpdateAsync(serviceOrder);
        }

        public async Task ProcessApprovalDecisionAsync(
            ApprovalDecisionRequest request,
            Guid changedById
        )
        {
            var serviceOrder =
                await _repository.GetByIdAsync(request.ServiceOrderId)
                ?? throw new DomainException("Service order not found.");

            if (request.IsApproved)
            {
                serviceOrder.Approve();
            }
            else
            {
                serviceOrder.Reject();

                if (!serviceOrder.Supplies.IsNullOrEmpty())
                {
                    var suppliesToRestock = serviceOrder
                        .Supplies.Select(s => (s.SupplyId, s.Quantity))
                        .ToList();

                    await _supplyService.RestockSuppliesAsync(suppliesToRestock);
                }
            }

            serviceOrder.AddHistory(serviceOrder.Status, changedById);

            await _repository.UpdateAsync(serviceOrder);
        }
    }
}
