using AutoRepairShop.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Tests.Repositories;

internal sealed class SqliteRepositoryTestContext : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    private SqliteRepositoryTestContext(
        SqliteConnection connection,
        DbContextOptions<AppDbContext> options
    )
    {
        _connection = connection;
        _options = options;
    }

    public static async Task<SqliteRepositoryTestContext> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return new SqliteRepositoryTestContext(connection, options);
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
