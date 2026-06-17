using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Models.Supply;
using AutoRepairShop.Infrastructure.Data.Mappings;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class SupplyRepositoryTests
{
    [Fact]
    public async Task AddAsync_WhenSupplyIsValid_ShouldPersistEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var entity = new Supply("Filtro", 45m, 10);

        await using (var context = database.CreateDbContext())
        {
            var repository = new SupplyRepository(context);
            await repository.AddAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Supplies.FindAsync(entity.Id);

        Assert.NotNull(persisted);
        Assert.Equal(10, persisted.StockQuantity);
    }

    [Fact]
    public async Task GetSuppliesInStockAsync_WhenSomeSuppliesHaveInsufficientStock_ShouldReturnOnlyAvailableSuppliesAndDecreaseTrackedStock()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var available = new Supply("Filtro", 45m, 10);
        var unavailable = new Supply("Oleo", 70m, 1);

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.Supplies.AddRange(available.ToEntity(), unavailable.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new SupplyRepository(context);

        var result = await repository.GetSuppliesInStockAsync([
            new SupplyRequestItem(available.Id, 3),
            new SupplyRequestItem(unavailable.Id, 2),
        ]);

        Assert.Single(result);
        Assert.Equal(available.Id, result.Single().Id);
        Assert.Equal(7, result.Single().StockQuantity);
        Assert.Equal(7, (await repository.GetByIdAsync(available.Id))!.StockQuantity);
        Assert.Equal(1, (await repository.GetByIdAsync(unavailable.Id))!.StockQuantity);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplyExists_ShouldPersistChanges()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Supply entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Supply("Filtro", 45m, 10);
            seedContext.Supplies.Add(entity.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new SupplyRepository(updateContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            persisted.Update("Oleo", 70m, 15);
            await repository.UpdateAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        var updated = await assertionContext.Supplies.FindAsync(entity.Id);

        Assert.NotNull(updated);
        Assert.Equal("Oleo", updated.Name);
        Assert.Equal(15, updated.StockQuantity);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplyExists_ShouldRemoveEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Supply entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Supply("Filtro", 45m, 10);
            seedContext.Supplies.Add(entity.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using (var deleteContext = database.CreateDbContext())
        {
            var repository = new SupplyRepository(deleteContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            await repository.DeleteAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        Assert.Null(await assertionContext.Supplies.FindAsync(entity.Id));
    }
}
