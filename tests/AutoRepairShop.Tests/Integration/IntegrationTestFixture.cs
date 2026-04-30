using AutoMapper;
using AutoRepairShop.Application.Interfaces;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Application.Mapping;
using AutoRepairShop.Application.Security;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Repositories;
using AutoRepairShop.Tests.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRepairShop.Tests.Integration;

public sealed class IntegrationTestFixture : IAsyncDisposable
{
    private readonly SqliteRepositoryTestContext _dbContext;
    private readonly ServiceProvider _serviceProvider;

    private IntegrationTestFixture(
        SqliteRepositoryTestContext dbContext,
        ServiceProvider serviceProvider
    )
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public static async Task<IntegrationTestFixture> CreateAsync()
    {
        var dbContext = await SqliteRepositoryTestContext.CreateAsync();

        var services = new ServiceCollection();

        // AutoMapper
        services.AddAutoMapper(typeof(MapperProfile).Assembly);

        // Repositories
        services.AddScoped(_ => dbContext.CreateDbContext());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ISupplyRepository, SupplyRepository>();
        services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ISupplyService, SupplyService>();
        services.AddScoped<IServiceOrderService, ServiceOrderService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var serviceProvider = services.BuildServiceProvider();

        return new IntegrationTestFixture(dbContext, serviceProvider);
    }

    public AppDbContext CreateDbContext() => _dbContext.CreateDbContext();

    public T GetService<T>()
        where T : notnull => _serviceProvider.GetRequiredService<T>();

    public async Task SeedServicesAsync(params Service[] services)
    {
        using var context = CreateDbContext();
        context.Services.AddRange(services);
        await context.SaveChangesAsync();
    }

    public async Task SeedSuppliesAsync(params Supply[] supplies)
    {
        using var context = CreateDbContext();
        context.Supplies.AddRange(supplies);
        await context.SaveChangesAsync();
    }

    public async Task<Guid> GetSeededCustomerId()
    {
        using var context = CreateDbContext();
        var customer = await context.Customers.FindAsync(
            Guid.Parse("11111111-1111-1111-1111-111111111111")
        );
        return customer?.Id ?? throw new InvalidOperationException("Demo customer not found");
    }

    public async Task<Guid> GetSeededAdminId()
    {
        using var context = CreateDbContext();
        var admin = await context.Admins.FindAsync(
            Guid.Parse("22222222-2222-2222-2222-222222222222")
        );
        return admin?.Id ?? throw new InvalidOperationException("Demo admin not found");
    }

    public async ValueTask DisposeAsync()
    {
        _serviceProvider.Dispose();
        await _dbContext.DisposeAsync();
    }
}
