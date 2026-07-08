using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace AutoRepairShop.Tests.Integration;

internal sealed class SqlServerIntegrationTestContext : IAsyncDisposable
{
    private readonly MsSqlContainer _container;
    private readonly DbContextOptions<AppDbContext> _options;

    private SqlServerIntegrationTestContext(
        MsSqlContainer container,
        DbContextOptions<AppDbContext> options
    )
    {
        _container = container;
        _options = options;
    }

    public static async Task<SqlServerIntegrationTestContext> CreateAsync()
    {
        var container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_strong_Password123!")
            .Build();

        await container.StartAsync();

        var connectionString = container.GetConnectionString();
        if (!connectionString.Contains("TrustServerCertificate", StringComparison.OrdinalIgnoreCase))
        {
            connectionString = $"{connectionString};TrustServerCertificate=True";
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();

        return new SqlServerIntegrationTestContext(container, options);
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
