using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class VehicleRepositoryTests
{
    [Fact]
    public async Task AddAsync_WhenVehicleIsValid_ShouldPersistEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var entity = new Vehicle(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );

        await using (var context = database.CreateDbContext())
        {
            var repository = new VehicleRepository(context);
            await repository.AddAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Vehicles.FindAsync(entity.Id);

        Assert.NotNull(persisted);
        Assert.Equal("ABC1234", persisted.Plate.Value);
    }

    [Fact]
    public async Task GetByPlateAsync_WhenPlateExists_ShouldNormalizeAndReturnVehicle()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var entity = new Vehicle(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.Vehicles.Add(entity);
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new VehicleRepository(context);

        var result = await repository.GetByPlateAsync("abc-1234");

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenVehicleExists_ShouldPersistChanges()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Vehicle entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Vehicle(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                VehiclePlate.Create("ABC1234"),
                "Ford",
                "Ka",
                2022
            );
            seedContext.Vehicles.Add(entity);
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new VehicleRepository(updateContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            persisted.Update("Fiat", "Uno", 2023);
            await repository.UpdateAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        var updated = await assertionContext.Vehicles.FindAsync(entity.Id);

        Assert.NotNull(updated);
        Assert.Equal("Fiat", updated.Brand);
        Assert.Equal("Uno", updated.Model);
        Assert.Equal(2023, updated.Year);
    }

    [Fact]
    public async Task DeleteAsync_WhenVehicleExists_ShouldRemoveEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Vehicle entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Vehicle(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                VehiclePlate.Create("ABC1234"),
                "Ford",
                "Ka",
                2022
            );
            seedContext.Vehicles.Add(entity);
            await seedContext.SaveChangesAsync();
        }

        await using (var deleteContext = database.CreateDbContext())
        {
            var repository = new VehicleRepository(deleteContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            await repository.DeleteAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        Assert.Null(await assertionContext.Vehicles.FindAsync(entity.Id));
    }
}
