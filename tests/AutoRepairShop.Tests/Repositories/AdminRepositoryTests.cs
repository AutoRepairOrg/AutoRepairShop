using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class AdminRepositoryTests
{
    [Fact]
    public async Task GetByUserNameAsync_WhenAdminExists_ShouldReturnSeededAdmin()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        await using var context = database.CreateDbContext();
        var repository = new AdminRepository(context);

        var result = await repository.GetByUserNameAsync("admin");

        Assert.NotNull(result);
        Assert.Equal("Admin Master", result.Name);
        Assert.Equal("Sistema", result.Department);
    }

    [Fact]
    public async Task CrudMethods_WhenCalled_ShouldThrowNotImplementedException()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        await using var context = database.CreateDbContext();
        var repository = new AdminRepository(context);
        var admin = new Admin("Admin", "TI", "admin2", "hash");

        await Assert.ThrowsAsync<NotImplementedException>(() => repository.AddAsync(admin));
        await Assert.ThrowsAsync<NotImplementedException>(() => repository.UpdateAsync(admin));
        await Assert.ThrowsAsync<NotImplementedException>(() => repository.DeleteAsync(admin));
        await Assert.ThrowsAsync<NotImplementedException>(() => repository.GetAllAsync());
        await Assert.ThrowsAsync<NotImplementedException>(() => repository.GetByIdAsync(admin.Id));
    }
}
