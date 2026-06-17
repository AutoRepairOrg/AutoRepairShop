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
    public async Task CrudMethods_WhenCalled_ShouldPersistAndQueryAdmin()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var admin = new Admin("Admin", "TI", "admin2", "hash");

        await using (var writeContext = database.CreateDbContext())
        {
            var repository = new AdminRepository(writeContext);
            await repository.AddAsync(admin);
        }

        await using (var readContext = database.CreateDbContext())
        {
            var repository = new AdminRepository(readContext);

            var byId = await repository.GetByIdAsync(admin.Id);
            Assert.NotNull(byId);
            Assert.Equal("admin2", byId.Username);

            var all = await repository.GetAllAsync();
            Assert.Contains(all, x => x.Id == admin.Id);
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new AdminRepository(updateContext);
            var persisted = await repository.GetByIdAsync(admin.Id);
            Assert.NotNull(persisted);
            var updated = Admin.Restore(
                persisted.Id,
                "Admin Atualizado",
                "Ops",
                persisted.Username,
                "new-hash"
            );
            await repository.UpdateAsync(updated);
        }

        await using (var deleteContext = database.CreateDbContext())
        {
            var repository = new AdminRepository(deleteContext);
            var persisted = await repository.GetByIdAsync(admin.Id);
            Assert.NotNull(persisted);
            await repository.DeleteAsync(persisted);
        }

        await using var assertionContext = database.CreateDbContext();
        Assert.Null(await assertionContext.Admins.FindAsync(admin.Id));
    }
}
