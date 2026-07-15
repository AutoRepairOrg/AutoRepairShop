using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Data.Mappings;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class CustomerRepositoryTests
{
    [Fact]
    public async Task AddAsync_WhenCustomerIsValid_ShouldPersistEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var customer = new Customer(
            "Maria",
            Document.Create("11144477735"),
            "11999999999",
            "maria@email.com",
            "maria",
            "hash"
        );

        await using (var context = database.CreateDbContext())
        {
            var repository = new CustomerRepository(context);
            await repository.AddAsync(customer);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(persisted);
        Assert.Equal("maria", persisted.Username);
    }

    [Fact]
    public async Task GetByUserNameAsync_WhenCustomerExists_ShouldReturnMatchingEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        await using var context = database.CreateDbContext();
        var repository = new CustomerRepository(context);

        var result = await repository.GetByUserNameAsync("customer");

        Assert.NotNull(result);
        Assert.Equal("Cliente Demo", result.Name);
    }

    [Fact]
    public async Task GetByCpfCnpjAsync_WhenDocumentIsFormatted_ShouldNormalizeAndReturnCustomer()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        await using var context = database.CreateDbContext();
        var repository = new CustomerRepository(context);

        var result = await repository.GetByCpfCnpjAsync("529.982.247-25");

        Assert.NotNull(result);
        Assert.Equal("customer", result.Username);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_ShouldPersistChanges()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Customer customer;

        await using (var seedContext = database.CreateDbContext())
        {
            customer = new Customer(
                "Maria",
                Document.Create("11144477735"),
                "11999999999",
                "maria@email.com",
                "maria",
                "hash"
            );
            seedContext.Customers.Add(customer.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new CustomerRepository(updateContext);
            var entity = await repository.GetByIdAsync(customer.Id);
            Assert.NotNull(entity);
            entity.Update("Maria Silva", "11888888888", "maria.silva@email.com", "maria.silva", "new-hash");
            await repository.UpdateAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Customers.FindAsync(customer.Id);

        Assert.NotNull(persisted);
        Assert.Equal("Maria Silva", persisted.Name);
        Assert.Equal("11888888888", persisted.Phone);
        Assert.Equal("maria.silva", persisted.Username);
        Assert.Equal("new-hash", persisted.Password);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ShouldRemoveEntity()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        Customer customer;

        await using (var seedContext = database.CreateDbContext())
        {
            customer = new Customer(
                "Maria",
                Document.Create("11144477735"),
                "11999999999",
                "maria@email.com",
                "maria",
                "hash"
            );
            seedContext.Customers.Add(customer.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using (var deleteContext = database.CreateDbContext())
        {
            var repository = new CustomerRepository(deleteContext);
            var entity = await repository.GetByIdAsync(customer.Id);
            Assert.NotNull(entity);
            await repository.DeleteAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.Customers.FindAsync(customer.Id);

        Assert.Null(persisted);
    }
}
