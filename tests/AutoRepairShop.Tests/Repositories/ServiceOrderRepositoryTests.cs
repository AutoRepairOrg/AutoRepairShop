using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Tests.Repositories;

public class ServiceOrderRepositoryTests
{
    private static readonly Guid CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task AddAsync_WhenServiceOrderIsValid_ShouldPersistAggregateWithRelations()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var (vehicleId, serviceId, supplyId) = await SeedDependenciesAsync(database);
        var order = new ServiceOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = CustomerId,
            VehicleId = vehicleId,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
        };
        order.AddService(serviceId);
        order.AddSupply(supplyId, 2);
        order.AddHistory(order.Status, Guid.NewGuid());

        await using (var context = database.CreateDbContext())
        {
            var repository = new ServiceOrderRepository(context);
            await repository.AddAsync(order);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext
            .ServiceOrders.Include(x => x.Services)
            .Include(x => x.Supplies)
            .Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == order.Id);

        Assert.NotNull(persisted);
        Assert.Single(persisted.Services);
        Assert.Single(persisted.Supplies);
        Assert.Equal(2, persisted.Supplies.Single().Quantity);
        Assert.Single(persisted.History);
    }

    [Fact]
    public async Task GetByIdAsync_WhenServiceOrderExists_ShouldLoadServicesSuppliesAndHistory()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var (vehicleId, serviceId, supplyId) = await SeedDependenciesAsync(database);
        var orderId = Guid.NewGuid();

        await using (var seedContext = database.CreateDbContext())
        {
            var order = new ServiceOrder
            {
                Id = orderId,
                CustomerId = CustomerId,
                VehicleId = vehicleId,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
            };
            order.AddService(serviceId);
            order.AddSupply(supplyId, 1);
            order.AddHistory(ServiceOrderStatus.Received, Guid.NewGuid());
            order.StartDiagnosis();
            order.AddHistory(ServiceOrderStatus.InDiagnosis, Guid.NewGuid());
            seedContext.ServiceOrders.Add(order);
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new ServiceOrderRepository(context);

        var result = await repository.GetByIdAsync(orderId);

        Assert.NotNull(result);
        Assert.Single(result.Services);
        Assert.Single(result.Supplies);
        Assert.Equal(2, result.History.Count);
        Assert.Equal(ServiceOrderStatus.Received, result.History.First().Status);
        Assert.Equal(ServiceOrderStatus.InDiagnosis, result.History.Last().Status);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatusFilterIsProvided_ShouldReturnOnlyMatchingOrdersOrderedByCreatedAtDescending()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var (vehicleId, _, _) = await SeedDependenciesAsync(database);

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.ServiceOrders.AddRange(
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.Received,
                    CreatedAt = DateTime.UtcNow.AddHours(-3),
                },
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.InDiagnosis,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                },
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.InDiagnosis,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                }
            );
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new ServiceOrderRepository(context);

        var result = await repository.GetAllAsync(ServiceOrderStatus.InDiagnosis);

        Assert.Equal(2, result.Count);
        Assert.All(result, order => Assert.Equal(ServiceOrderStatus.InDiagnosis, order.Status));
        Assert.True(result[0].CreatedAt >= result[1].CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_WhenStatusChanges_ShouldPersistUpdatedFieldsAndLatestHistory()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var (vehicleId, _, _) = await SeedDependenciesAsync(database);
        var orderId = Guid.NewGuid();

        await using (var seedContext = database.CreateDbContext())
        {
            var order = new ServiceOrder
            {
                Id = orderId,
                CustomerId = CustomerId,
                VehicleId = vehicleId,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
            };
            order.AddHistory(ServiceOrderStatus.Received, Guid.NewGuid());
            seedContext.ServiceOrders.Add(order);
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new ServiceOrderRepository(updateContext);
            var persisted = await repository.GetByIdAsync(orderId);
            Assert.NotNull(persisted);
            persisted.StartDiagnosis();
            persisted.AddHistory(persisted.Status, Guid.NewGuid());
            await repository.UpdateAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        var updated = await assertionContext.ServiceOrders.FindAsync(orderId);
        var history = assertionContext
            .ServiceOrderHistories.Where(x => x.ServiceOrderId == orderId)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        Assert.NotNull(updated);
        Assert.Equal(ServiceOrderStatus.InDiagnosis, updated.Status);
        Assert.Equal(2, history.Count);
        Assert.Equal(ServiceOrderStatus.InDiagnosis, history.Last().Status);
    }

    [Fact]
    public async Task GetAverageExecutionTimeAsync_WhenCompletedOrdersExist_ShouldReturnCalculatedMetrics()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var (vehicleId, _, _) = await SeedDependenciesAsync(database);
        var now = DateTime.UtcNow;

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.ServiceOrders.AddRange(
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.Finished,
                    CreatedAt = now.AddDays(-3),
                    StartedAt = now.AddHours(-30),
                    FinishedAt = now.AddHours(-6),
                },
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.Delivered,
                    CreatedAt = now.AddDays(-2),
                    StartedAt = now.AddHours(-20),
                    FinishedAt = now.AddHours(-2),
                },
                new ServiceOrder
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerId,
                    VehicleId = vehicleId,
                    Status = ServiceOrderStatus.Received,
                    CreatedAt = now.AddDays(-1),
                }
            );
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new ServiceOrderRepository(context);

        var result = await repository.GetAverageExecutionTimeAsync();

        Assert.Equal(3, result.total);
        Assert.Equal(2, result.completed);
        Assert.Equal(21, result.averageHours);
        Assert.NotNull(result.earliest);
        Assert.NotNull(result.latest);
    }

    private static async Task<(
        Guid VehicleId,
        Guid ServiceId,
        Guid SupplyId
    )> SeedDependenciesAsync(SqliteRepositoryTestContext database)
    {
        var vehicleId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var supplyId = Guid.NewGuid();

        await using var context = database.CreateDbContext();
        if (await context.Vehicles.FindAsync(vehicleId) is not null)
            return (vehicleId, serviceId, supplyId);

        context.Vehicles.Add(
            new Vehicle(
                CustomerId,
                AutoRepairShop.Domain.ValueObjects.VehiclePlate.Create("ABC1234"),
                "Ford",
                "Ka",
                2022
            )
        );
        context.Services.Add(new Service("Troca de oleo", "Descricao", 150m));
        context.Supplies.Add(new Supply("Filtro", 45m, 10));
        await context.SaveChangesAsync();

        var vehicle = await context.Vehicles.OrderByDescending(x => x.Id).FirstAsync();
        var service = await context.Services.OrderByDescending(x => x.Id).FirstAsync();
        var supply = await context.Supplies.OrderByDescending(x => x.Id).FirstAsync();

        return (vehicle.Id, service.Id, supply.Id);
    }
}
