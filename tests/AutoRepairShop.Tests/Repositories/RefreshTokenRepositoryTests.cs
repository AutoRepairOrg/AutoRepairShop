using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Infrastructure.Data.Mappings;
using AutoRepairShop.Infrastructure.Repositories;

namespace AutoRepairShop.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    [Fact]
    public async Task SaveAsync_WhenTokenIsValid_ShouldPersistAndGetByTokenAsyncShouldReturnIt()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        var token = new RefreshToken(Guid.NewGuid(), "refresh-token", DateTime.UtcNow.AddDays(1));

        await using (var context = database.CreateDbContext())
        {
            var repository = new RefreshTokenRepository(context);
            await repository.SaveAsync(token);
        }

        await using var assertionContext = database.CreateDbContext();
        var repositoryAssertion = new RefreshTokenRepository(assertionContext);
        var persisted = await repositoryAssertion.GetByTokenAsync("refresh-token");

        Assert.NotNull(persisted);
        Assert.Equal(token.UserId, persisted.UserId);
        Assert.False(persisted.IsRevoked);
    }

    [Fact]
    public async Task UpdateAsync_WhenTokenIsRevoked_ShouldPersistRevokedState()
    {
        await using var database = await SqliteRepositoryTestContext.CreateAsync();
        RefreshToken token;

        await using (var seedContext = database.CreateDbContext())
        {
            token = new RefreshToken(Guid.NewGuid(), "refresh-token", DateTime.UtcNow.AddDays(1));
            seedContext.RefreshTokens.Add(token.ToEntity());
            await seedContext.SaveChangesAsync();
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var repository = new RefreshTokenRepository(updateContext);
            var entity = await repository.GetByTokenAsync(token.Token);
            Assert.NotNull(entity);
            entity.Revoke();
            await repository.UpdateAsync(entity);
        }

        await using var assertionContext = database.CreateDbContext();
        var persisted = await assertionContext.RefreshTokens.FindAsync(token.Id);

        Assert.NotNull(persisted);
        Assert.True(persisted.IsRevoked);
    }
}
