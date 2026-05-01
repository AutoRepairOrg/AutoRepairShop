using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class ServiceRepositoryTests
{
    [Fact]
    public async Task AddAsync_WhenServiceIsValid_ShouldPersistEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var entity = new Service("Troca de oleo", "Descricao", 150m);

        await using (var context = database.CreateDbContext())
        {
            var repository = new ServiceRepository(context);
            await repository.AddAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Services.FindAsync(entity.Id);

        Assert.NotNull(persisted);
        Assert.Equal("Troca de oleo", persisted.Name);
    }

    [Fact]
    public async Task GetServicesByIdsAsync_WhenIdsExist_ShouldReturnOnlyRequestedServices()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var first = new Service("Troca de oleo", "Descricao", 150m);
        var second = new Service("Balanceamento", "Descricao", 200m);
        var third = new Service("Alinhamento", "Descricao", 250m);

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.Services.AddRange(first, second, third);
            await seedContext.SaveChangesAsync();
        }

        await using var context = database.CreateDbContext();
        var repository = new ServiceRepository(context);

        var result = await repository.GetServicesByIdsAsync([first.Id, third.Id]);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, service => service.Id == first.Id);
        Assert.Contains(result, service => service.Id == third.Id);
        Assert.DoesNotContain(result, service => service.Id == second.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenServiceExists_ShouldPersistChanges()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Service entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Service("Troca de oleo", "Descricao", 150m);
            seedContext.Services.Add(entity);
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new ServiceRepository(updateContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            persisted.Update("Balanceamento", "Descricao atualizada", 220m);
            await repository.UpdateAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        var updated = await assertionContext.Services.FindAsync(entity.Id);

        Assert.NotNull(updated);
        Assert.Equal("Balanceamento", updated.Name);
        Assert.Equal(220m, updated.Price);
    }

    [Fact]
    public async Task DeleteAsync_WhenServiceExists_ShouldRemoveEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Service entity;

        await using (var seedContext = database.CreateDbContext())
        {
            entity = new Service("Troca de oleo", "Descricao", 150m);
            seedContext.Services.Add(entity);
            await seedContext.SaveChangesAsync();
        }

        await using (var deleteContext = database.CreateDbContext())
        {
            var repository = new ServiceRepository(deleteContext);
            var persisted = await repository.GetByIdAsync(entity.Id);
            Assert.NotNull(persisted);
            await repository.DeleteAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        Assert.Null(await assertionContext.Services.FindAsync(entity.Id));
    }
}
