using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Infrastructure.Data;

namespace AutoRepairShop.Tests.Integration;

public class ServiceOrderCreationIntegrationTests : IAsyncLifetime
{
    private IntegrationTestFixture _fixture = null!;

    public async Task InitializeAsync()
    {
        _fixture = await IntegrationTestFixture.CreateAsync();
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithValidServicesAndSupplies_ShouldPersistOrderSuccessfully()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Troca de óleo", "Troca do óleo do motor", 150m);
        var supply = new Supply("Filtro de óleo", 45m, 10);

        await _fixture.SeedServicesAsync(service);
        await _fixture.SeedSuppliesAsync(supply);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "ABC1234",
                Brand = "Ford",
                Model = "Ka",
                Year = 2022,
            },
            ServiceIds = [service.Id],
            SupplyItems = [new SupplyItemDto { SupplyId = supply.Id, Quantity = 2 }],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act
        await serviceOrderService.CreateServiceOrderAsync(request, adminId);

        // Assert
        using var context = _fixture.CreateDbContext();
        var createdOrder = context.ServiceOrders.FirstOrDefault(o => o.CustomerId == customerId);

        Assert.NotNull(createdOrder);
        Assert.Equal(customerId, createdOrder.CustomerId);
        Assert.Single(createdOrder.Services);
        Assert.Single(createdOrder.Supplies);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithNewVehicle_ShouldCreateVehicleAutomatically()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Diagnóstico", "Diagnóstico completo", 100m);
        await _fixture.SeedServicesAsync(service);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "XYZ9999",
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2021,
            },
            ServiceIds = [service.Id],
            SupplyItems = [],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act
        await serviceOrderService.CreateServiceOrderAsync(request, adminId);

        // Assert
        using var context = _fixture.CreateDbContext();
        var createdVehicle = context.Vehicles.FirstOrDefault(v =>
            v.CustomerId == customerId && v.Brand == "Toyota"
        );

        Assert.NotNull(createdVehicle);
        Assert.Equal("XYZ9999", createdVehicle.Plate);
        Assert.Equal("Toyota", createdVehicle.Brand);
        Assert.Equal("Corolla", createdVehicle.Model);
        Assert.Equal(2021, createdVehicle.Year);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithExistingVehicle_ShouldReuseExistingVehicle()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Revisão", "Revisão anual", 200m);
        await _fixture.SeedServicesAsync(service);

        // Cria primeiro veículo
        var request1 = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "REU1234",
                Brand = "Volkswagen",
                Model = "Gol",
                Year = 2020,
            },
            ServiceIds = [service.Id],
            SupplyItems = [],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act - Cria primeiro ServiceOrder
        await serviceOrderService.CreateServiceOrderAsync(request1, adminId);

        // Conta veículos antes
        using (var context = _fixture.CreateDbContext())
        {
            var vehicleCountBefore = context.Vehicles.Count(v =>
                v.CustomerId == customerId && v.Brand == "Volkswagen"
            );
            Assert.Equal(1, vehicleCountBefore);
        }

        // Cria segundo ServiceOrder com mesmo veículo
        var request2 = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "REU1234",
                Brand = "Volkswagen",
                Model = "Gol",
                Year = 2020,
            },
            ServiceIds = [service.Id],
            SupplyItems = [],
        };

        await serviceOrderService.CreateServiceOrderAsync(request2, adminId);

        // Assert - Veículo não foi duplicado
        using var contextAfter = _fixture.CreateDbContext();
        var vehicleCountAfter = contextAfter.Vehicles.Count(v =>
            v.CustomerId == customerId && v.Brand == "Volkswagen"
        );
        Assert.Equal(1, vehicleCountAfter);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithNeitherServicesNorSupplies_ShouldThrowDomainException()
    {
        // Arrange
        var adminId = await _fixture.GetSeededAdminId();

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "ERR0000",
                Brand = "Honda",
                Model = "Civic",
                Year = 2023,
            },
            ServiceIds = [],
            SupplyItems = [],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            serviceOrderService.CreateServiceOrderAsync(request, adminId)
        );

        Assert.Contains("No services or supplies", exception.Message);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithInvalidCustomerDocument_ShouldThrowDomainException()
    {
        // Arrange
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Limpeza", "Limpeza do veículo", 80m);
        await _fixture.SeedServicesAsync(service);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "99999999999",
            Vehicle = new VehicleDto
            {
                Plate = "INV0001",
                Brand = "Hyundai",
                Model = "HB20",
                Year = 2023,
            },
            ServiceIds = [service.Id],
            SupplyItems = [],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            serviceOrderService.CreateServiceOrderAsync(request, adminId)
        );

        Assert.Contains("Invalid CPF or CNPJ", exception.Message);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_ShouldCreateHistoryEntry()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Alinhamento", "Alinhamento e balanceamento", 120m);
        await _fixture.SeedServicesAsync(service);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "HIS0001",
                Brand = "Fiat",
                Model = "Uno",
                Year = 2019,
            },
            ServiceIds = [service.Id],
            SupplyItems = [],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act
        await serviceOrderService.CreateServiceOrderAsync(request, adminId);

        // Assert
        using var context = _fixture.CreateDbContext();
        var createdOrder = context.ServiceOrders.FirstOrDefault(o =>
            o.CustomerId == customerId && o.Status == ServiceOrderStatus.Received
        );
        Assert.NotNull(createdOrder);

        var historyEntry = context.ServiceOrderHistories.FirstOrDefault(h =>
            h.ServiceOrderId == createdOrder.Id
        );
        Assert.NotNull(historyEntry);
        Assert.Equal(ServiceOrderStatus.Received, historyEntry.Status);
        Assert.Equal(adminId, historyEntry.CreatedById);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_WithMultipleServicesAndSupplies_ShouldPersistAllItems()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service1 = new Service("Serviço 1", "Descrição 1", 100m);
        var service2 = new Service("Serviço 2", "Descrição 2", 150m);
        var service3 = new Service("Serviço 3", "Descrição 3", 200m);

        var supply1 = new Supply("Peça 1", 50m, 20);
        var supply2 = new Supply("Peça 2", 75m, 15);

        await _fixture.SeedServicesAsync(service1, service2, service3);
        await _fixture.SeedSuppliesAsync(supply1, supply2);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "MUL0001",
                Brand = "Chevrolet",
                Model = "Onix",
                Year = 2021,
            },
            ServiceIds = [service1.Id, service2.Id, service3.Id],
            SupplyItems =
            [
                new SupplyItemDto { SupplyId = supply1.Id, Quantity = 3 },
                new SupplyItemDto { SupplyId = supply2.Id, Quantity = 5 },
            ],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act
        await serviceOrderService.CreateServiceOrderAsync(request, adminId);

        // Assert
        using var context = _fixture.CreateDbContext();
        var createdOrder = context.ServiceOrders.FirstOrDefault(o =>
            o.CustomerId == customerId && o.Status == ServiceOrderStatus.Received
        );

        Assert.NotNull(createdOrder);
        Assert.Equal(3, createdOrder.Services.Count);
        Assert.Equal(2, createdOrder.Supplies.Count);
    }

    [Fact]
    public async Task CreateServiceOrderAsync_ShouldDecrementSupplyStock()
    {
        // Arrange
        var customerId = await _fixture.GetSeededCustomerId();
        var adminId = await _fixture.GetSeededAdminId();

        var service = new Service("Manutenção", "Manutenção preventiva", 250m);
        var supply = new Supply("Óleo sintético", 80m, 10);

        await _fixture.SeedServicesAsync(service);
        await _fixture.SeedSuppliesAsync(supply);

        var request = new CreateServiceOrderRequest
        {
            CustomerDocument = "52998224725",
            Vehicle = new VehicleDto
            {
                Plate = "STO0001",
                Brand = "Renault",
                Model = "Sandero",
                Year = 2020,
            },
            ServiceIds = [service.Id],
            SupplyItems = [new SupplyItemDto { SupplyId = supply.Id, Quantity = 3 }],
        };

        var serviceOrderService = _fixture.GetService<IServiceOrderService>();

        // Act
        await serviceOrderService.CreateServiceOrderAsync(request, adminId);

        // Assert
        using var context = _fixture.CreateDbContext();
        var updatedSupply = context.Supplies.FirstOrDefault(s => s.Id == supply.Id);

        Assert.NotNull(updatedSupply);
        Assert.Equal(7, updatedSupply.StockQuantity); // 10 - 3
    }
}
