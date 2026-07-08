using AutoMapper;
using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.DTOs.ServiceOrder.Response;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.ValueObjects;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class ServiceOrderServiceTests
{
    private readonly Mock<IServiceOrderRepository> _repositoryMock = new();
    private readonly Mock<ICustomerService> _customerServiceMock = new();
    private readonly Mock<IVehicleService> _vehicleServiceMock = new();
    private readonly Mock<IServiceService> _serviceServiceMock = new();
    private readonly Mock<ISupplyService> _supplyServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateServiceOrderAsync_WhenServicesOrSuppliesExist_ShouldPersistOrderWithHistory()
    {
        var createdById = Guid.NewGuid();
        var customer = new Customer(
            "Maria",
            Document.Create("12345678909"),
            "11999999999",
            "maria",
            "hash"
        );
        var vehicle = new Vehicle(
            customer.Id,
            AutoRepairShop.Domain.ValueObjects.VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );
        var service = new Service("Troca de oleo", "Descricao", 150m);
        var supply = new Supply("Filtro", 45m, 10);
        var request = CreateServiceOrderRequest(service.Id, supply.Id);
        _customerServiceMock
            .Setup(serviceMock => serviceMock.GetByCpfCnpjAsync(request.CustomerDocument))
            .ReturnsAsync(customer);
        _vehicleServiceMock
            .Setup(serviceMock => serviceMock.GetOrCreateAsync(request.Vehicle, customer.Id))
            .ReturnsAsync(vehicle);
        _serviceServiceMock
            .Setup(serviceMock => serviceMock.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([service]);
        _supplyServiceMock
            .Setup(serviceMock => serviceMock.GetSuppliesInStockAsync(request.SupplyItems))
            .ReturnsAsync([supply]);
        var sut = CreateSut();

        await sut.CreateServiceOrderAsync(request, createdById);

        _repositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.Is<ServiceOrder>(order =>
                        order.CustomerId == customer.Id
                        && order.VehicleId == vehicle.Id
                        && order.Services.Count == 1
                        && order.Services.Single().ServiceId == service.Id
                        && order.Supplies.Count == 1
                        && order.Supplies.Single().SupplyId == supply.Id
                        && order.Supplies.Single().Quantity == request.SupplyItems.Single().Quantity
                        && order.History.Count == 1
                        && order.History.Single().Status == ServiceOrderStatus.Received
                        && order.History.Single().CreatedById == createdById
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WhenNoServicesOrSuppliesAreAvailable_ShouldThrowDomainException()
    {
        var customer = new Customer(
            "Maria",
            Document.Create("12345678909"),
            "11999999999",
            "maria",
            "hash"
        );
        var vehicle = new Vehicle(
            customer.Id,
            AutoRepairShop.Domain.ValueObjects.VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );
        var request = CreateServiceOrderRequest(Guid.NewGuid(), Guid.NewGuid());
        _customerServiceMock
            .Setup(serviceMock => serviceMock.GetByCpfCnpjAsync(request.CustomerDocument))
            .ReturnsAsync(customer);
        _vehicleServiceMock
            .Setup(serviceMock => serviceMock.GetOrCreateAsync(request.Vehicle, customer.Id))
            .ReturnsAsync(vehicle);
        _serviceServiceMock
            .Setup(serviceMock => serviceMock.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([]);
        _supplyServiceMock
            .Setup(serviceMock => serviceMock.GetSuppliesInStockAsync(request.SupplyItems))
            .ReturnsAsync([]);
        var sut = CreateSut();

        var action = () => sut.CreateServiceOrderAsync(request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("No services or supplies available for the service order.", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldMapRepositoryResult()
    {
        var serviceOrder = new ServiceOrder { Id = Guid.NewGuid() };
        var response = new GetServiceOrderResponse { Id = serviceOrder.Id };
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(serviceOrder.Id))
            .ReturnsAsync(serviceOrder);
        _mapperMock
            .Setup(mapper => mapper.Map<GetServiceOrderResponse>(serviceOrder))
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.GetByIdAsync(serviceOrder.Id);

        Assert.Same(response, result);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsOrders_ShouldMapAndReturnResponses()
    {
        var status = ServiceOrderStatus.InDiagnosis;
        var serviceOrders = new List<ServiceOrder>
        {
            new() { Id = Guid.NewGuid(), Status = status },
        };
        var mappedResponse = new List<GetServiceOrderResponse>
        {
            new() { Id = serviceOrders[0].Id, Status = status },
        };

        _repositoryMock
            .Setup(repository => repository.GetAllAsync(status))
            .ReturnsAsync(serviceOrders);
        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<GetServiceOrderResponse>>(serviceOrders))
            .Returns(mappedResponse);
        var sut = CreateSut();

        var result = await sut.GetAllAsync(status);

        Assert.Same(mappedResponse, result);
        _repositoryMock.Verify(repository => repository.GetAllAsync(status), Times.Once);
    }

    [Fact]
    public async Task AdvanceStatusAsync_WhenOrderIsReceived_ShouldMoveToDiagnosisAndPersist()
    {
        var changedById = Guid.NewGuid();
        var serviceOrder = new ServiceOrder { Id = Guid.NewGuid() };
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(serviceOrder.Id))
            .ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        await sut.AdvanceStatusAsync(serviceOrder.Id, changedById);

        Assert.Equal(ServiceOrderStatus.InDiagnosis, serviceOrder.Status);
        Assert.Equal(changedById, serviceOrder.History.Last().CreatedById);
        _repositoryMock.Verify(repository => repository.UpdateAsync(serviceOrder), Times.Once);
    }

    [Fact]
    public async Task AdvanceStatusAsync_WhenOrderIsWaitingApproval_ShouldThrowDomainException()
    {
        var serviceOrder = new ServiceOrder
        {
            Id = Guid.NewGuid(),
            Status = ServiceOrderStatus.WaitingApproval,
        };
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(serviceOrder.Id))
            .ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () => sut.AdvanceStatusAsync(serviceOrder.Id, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal(
            "Only customer can decide while status is waiting approval.",
            exception.Message
        );
    }

    [Fact]
    public async Task ProcessApprovalDecisionAsync_WhenApproved_ShouldMoveOrderToExecution()
    {
        var changedById = Guid.NewGuid();
        var request = new ApprovalDecisionRequest
        {
            ServiceOrderId = Guid.NewGuid(),
            IsApproved = true,
        };
        var serviceOrder = new ServiceOrder
        {
            Id = request.ServiceOrderId,
            Status = ServiceOrderStatus.WaitingApproval,
        };
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ServiceOrderId))
            .ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        await sut.ProcessApprovalDecisionAsync(request, changedById);

        Assert.Equal(ServiceOrderStatus.InExecution, serviceOrder.Status);
        Assert.NotNull(serviceOrder.StartedAt);
        Assert.Equal(changedById, serviceOrder.History.Last().CreatedById);
        _repositoryMock.Verify(repository => repository.UpdateAsync(serviceOrder), Times.Once);
    }

    [Fact]
    public async Task ProcessApprovalDecisionAsync_WhenRejectedAndOrderHasSupplies_ShouldRestockAndCancelOrder()
    {
        var changedById = Guid.NewGuid();
        var supplyId = Guid.NewGuid();
        var request = new ApprovalDecisionRequest
        {
            ServiceOrderId = Guid.NewGuid(),
            IsApproved = false,
        };
        var serviceOrder = new ServiceOrder
        {
            Id = request.ServiceOrderId,
            Status = ServiceOrderStatus.WaitingApproval,
        };
        serviceOrder.AddSupply(supplyId, 3);
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ServiceOrderId))
            .ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        await sut.ProcessApprovalDecisionAsync(request, changedById);

        Assert.Equal(ServiceOrderStatus.Canceled, serviceOrder.Status);
        _supplyServiceMock.Verify(
            service =>
                service.RestockSuppliesAsync(
                    It.Is<List<(Guid SupplyId, int Quantity)>>(supplies =>
                        supplies.Count == 1
                        && supplies[0].SupplyId == supplyId
                        && supplies[0].Quantity == 3
                    )
                ),
            Times.Once
        );
        _repositoryMock.Verify(repository => repository.UpdateAsync(serviceOrder), Times.Once);
    }

    [Fact]
    public async Task GetAverageExecutionTimeAsync_WhenRepositoryReturnsMetrics_ShouldRoundValues()
    {
        var earliest = DateTime.UtcNow.AddDays(-3);
        var latest = DateTime.UtcNow;
        _repositoryMock
            .Setup(repository => repository.GetAverageExecutionTimeAsync())
            .ReturnsAsync((10, 8, 49.555, earliest, latest));
        var sut = CreateSut();

        var result = await sut.GetAverageExecutionTimeAsync();

        Assert.Equal(10, result.TotalServiceOrders);
        Assert.Equal(8, result.CompletedServiceOrders);
        Assert.Equal(49.56, result.AverageExecutionTimeInHours);
        Assert.Equal(2.06, result.AverageExecutionTimeInDays);
        Assert.Equal(earliest, result.EarliestStartDate);
        Assert.Equal(latest, result.LatestFinishDate);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenOrderExistsInDiagnosisWithValidServices_ShouldUpdateItemsAndAdvanceStatus()
    {
        var changedById = Guid.NewGuid();
        var newService = new Service("Novo Serviço", "Descrição", 100m);
        var oldSupply = new Supply("Antigo Supply", 50m, 10);
        var newSupply = new Supply("Novo Supply", 60m, 10);

        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        serviceOrder.AddSupply(oldSupply.Id, 5);

        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [newService.Id],
            SupplyItems = [new SupplyItemDto { SupplyId = newSupply.Id, Quantity = 3 }],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        _serviceServiceMock
            .Setup(s => s.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([newService]);
        _supplyServiceMock
            .Setup(s => s.GetSuppliesInStockAsync(It.IsAny<List<SupplyItemDto>>()))
            .ReturnsAsync([newSupply]);
        var sut = CreateSut();

        await sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, changedById);

        Assert.Equal(ServiceOrderStatus.WaitingApproval, serviceOrder.Status);
        Assert.Single(serviceOrder.Services);
        Assert.Equal(newService.Id, serviceOrder.Services.First().ServiceId);
        Assert.Single(serviceOrder.Supplies);
        Assert.Equal(newSupply.Id, serviceOrder.Supplies.First().SupplyId);
        Assert.Equal(3, serviceOrder.Supplies.First().Quantity);
        Assert.Equal(changedById, serviceOrder.History.Last().CreatedById);
        _repositoryMock.Verify(r => r.UpdateAsync(serviceOrder), Times.Once);
        _supplyServiceMock.Verify(
            s =>
                s.RestockSuppliesAsync(
                    It.Is<List<(Guid, int)>>(list =>
                        list.Count == 1 && list[0].Item1 == oldSupply.Id && list[0].Item2 == 5
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenOrderNotFound_ShouldThrowDomainException()
    {
        var serviceOrderId = Guid.NewGuid();
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [Guid.NewGuid()],
            SupplyItems = [],
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(serviceOrderId))
            .ReturnsAsync((ServiceOrder?)null);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrderId, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("Service order not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenOrderNotInDiagnosis_ShouldThrowDomainException()
    {
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.Received };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [Guid.NewGuid()],
            SupplyItems = [],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("Only service orders in diagnosis can be edited by admin.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenRequestIsEmpty_ShouldThrowDomainException()
    {
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [],
            SupplyItems = [],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("No services or supplies available for the service order.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenServiceIdsHaveDuplicates_ShouldThrowDomainException()
    {
        var serviceId = Guid.NewGuid();
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [serviceId, serviceId],
            SupplyItems = [],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("Duplicate services are not allowed.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenSupplyIdsHaveDuplicates_ShouldThrowDomainException()
    {
        var supplyId = Guid.NewGuid();
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [Guid.NewGuid()],
            SupplyItems =
            [
                new SupplyItemDto { SupplyId = supplyId, Quantity = 1 },
                new SupplyItemDto { SupplyId = supplyId, Quantity = 2 },
            ],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("Duplicate supplies are not allowed.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenSupplyQuantityIsZeroOrNegative_ShouldThrowDomainException()
    {
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [Guid.NewGuid()],
            SupplyItems = [new SupplyItemDto { SupplyId = Guid.NewGuid(), Quantity = 0 }],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("Supply quantity must be greater than zero.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenOneOrMoreServicesNotFound_ShouldThrowDomainException()
    {
        var requestedServiceId = Guid.NewGuid();
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [requestedServiceId],
            SupplyItems = [],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        _serviceServiceMock
            .Setup(s => s.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([]);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("One or more services were not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenSuppliesDoNotHaveEnoughStock_ShouldThrowDomainException()
    {
        var service = new Service("Serviço", "Descrição", 100m);
        var supplyId = Guid.NewGuid();
        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [service.Id],
            SupplyItems = [new SupplyItemDto { SupplyId = supplyId, Quantity = 5 }],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        _serviceServiceMock
            .Setup(s => s.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([service]);
        _supplyServiceMock
            .Setup(s => s.GetSuppliesInStockAsync(It.IsAny<List<SupplyItemDto>>()))
            .ReturnsAsync([]);
        var sut = CreateSut();

        var action = () =>
            sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<DomainException>(action);
        Assert.Equal("One or more supplies do not have enough stock.", exception.Message);
    }

    [Fact]
    public async Task UpdateInDiagnosisAndAdvanceAsync_WhenPartiallyIncreasingSupplies_ShouldOnlyReserveNewQuantity()
    {
        var changedById = Guid.NewGuid();
        var service = new Service("Serviço", "Descrição", 100m);
        var supply = new Supply("Supply", 50m, 10);

        var serviceOrder = new ServiceOrder { Status = ServiceOrderStatus.InDiagnosis };
        serviceOrder.AddSupply(supply.Id, 3);

        var request = new UpdateServiceOrderInDiagnosisRequest
        {
            ServiceIds = [service.Id],
            SupplyItems = [new SupplyItemDto { SupplyId = supply.Id, Quantity = 5 }],
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(serviceOrder.Id)).ReturnsAsync(serviceOrder);
        _serviceServiceMock
            .Setup(s => s.GetServicesByIdsAsync(request.ServiceIds))
            .ReturnsAsync([service]);
        _supplyServiceMock
            .Setup(s =>
                s.GetSuppliesInStockAsync(
                    It.Is<List<SupplyItemDto>>(list =>
                        list.Count == 1 && list[0].SupplyId == supply.Id && list[0].Quantity == 2
                    )
                )
            )
            .ReturnsAsync([supply]);
        var sut = CreateSut();

        await sut.UpdateInDiagnosisAndAdvanceAsync(serviceOrder.Id, request, changedById);

        Assert.Equal(5, serviceOrder.Supplies.First().Quantity);
        _supplyServiceMock.Verify(
            s =>
                s.GetSuppliesInStockAsync(
                    It.Is<List<SupplyItemDto>>(list =>
                        list.Count == 1 && list[0].SupplyId == supply.Id && list[0].Quantity == 2
                    )
                ),
            Times.Once
        );
    }

    private AutoRepairShop.Application.Services.ServiceOrderService CreateSut()
    {
        return new AutoRepairShop.Application.Services.ServiceOrderService(
            _repositoryMock.Object,
            _customerServiceMock.Object,
            _vehicleServiceMock.Object,
            _serviceServiceMock.Object,
            _supplyServiceMock.Object,
            _mapperMock.Object
        );
    }

    private static CreateServiceOrderRequest CreateServiceOrderRequest(
        Guid serviceId,
        Guid supplyId
    )
    {
        return new CreateServiceOrderRequest
        {
            CustomerDocument = "12345678909",
            Vehicle = new VehicleDto
            {
                Plate = "ABC1234",
                Brand = "Ford",
                Model = "Ka",
                Year = 2022,
            },
            ServiceIds = [serviceId],
            SupplyItems = [new SupplyItemDto { SupplyId = supplyId, Quantity = 2 }],
        };
    }
}
