using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Data.Entities;

namespace AutoRepairShop.Infrastructure.Data.Mappings;

public static class PersistenceMappingExtensions
{
    public static CustomerEntity ToEntity(this Customer domain)
    {
        return new CustomerEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Document = domain.Document.Value,
            Phone = domain.Phone,
            Username = domain.Username,
            Password = domain.Password,
        };
    }

    public static Customer ToDomain(this CustomerEntity entity)
    {
        return Customer.Restore(
            entity.Id,
            entity.Name,
            Document.Create(entity.Document),
            entity.Phone,
            entity.Username,
            entity.Password
        );
    }

    public static AdminEntity ToEntity(this Admin domain)
    {
        return new AdminEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Department = domain.Department,
            Username = domain.Username,
            Password = domain.Password,
        };
    }

    public static Admin ToDomain(this AdminEntity entity)
    {
        return Admin.Restore(
            entity.Id,
            entity.Name,
            entity.Department,
            entity.Username,
            entity.Password
        );
    }

    public static VehicleEntity ToEntity(this Vehicle domain)
    {
        return new VehicleEntity
        {
            Id = domain.Id,
            CustomerId = domain.CustomerId,
            Plate = domain.Plate.Value,
            Brand = domain.Brand,
            Model = domain.Model,
            Year = domain.Year,
        };
    }

    public static Vehicle ToDomain(this VehicleEntity entity)
    {
        return Vehicle.Restore(
            entity.Id,
            entity.CustomerId,
            VehiclePlate.Create(entity.Plate),
            entity.Brand,
            entity.Model,
            entity.Year
        );
    }

    public static RefreshTokenEntity ToEntity(this RefreshToken domain)
    {
        return new RefreshTokenEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Token = domain.Token,
            ExpiresAt = domain.ExpiresAt,
            IsRevoked = domain.IsRevoked,
        };
    }

    public static RefreshToken ToDomain(this RefreshTokenEntity entity)
    {
        return RefreshToken.Restore(
            entity.Id,
            entity.UserId,
            entity.Token,
            entity.ExpiresAt,
            entity.IsRevoked
        );
    }

    public static ServiceEntity ToEntity(this Service domain)
    {
        return new ServiceEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Description = domain.Description,
            Price = domain.Price,
        };
    }

    public static Service ToDomain(this ServiceEntity entity)
    {
        return Service.Restore(entity.Id, entity.Name, entity.Description, entity.Price);
    }

    public static SupplyEntity ToEntity(this Supply domain)
    {
        return new SupplyEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Price = domain.Price,
            StockQuantity = domain.StockQuantity,
        };
    }

    public static Supply ToDomain(this SupplyEntity entity)
    {
        return Supply.Restore(entity.Id, entity.Name, entity.Price, entity.StockQuantity);
    }

    public static ServiceOrderEntity ToEntity(this ServiceOrder domain)
    {
        return new ServiceOrderEntity
        {
            Id = domain.Id,
            CustomerId = domain.CustomerId,
            VehicleId = domain.VehicleId,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            StartedAt = domain.StartedAt,
            FinishedAt = domain.FinishedAt,
            Services = [
                .. domain.Services.Select(item => new ServiceOrderServiceEntity
                {
                    ServiceOrderId = domain.Id,
                    ServiceId = item.ServiceId,
                }),
            ],
            Supplies = [
                .. domain.Supplies.Select(item => new ServiceOrderSupplyEntity
                {
                    ServiceOrderId = domain.Id,
                    SupplyId = item.SupplyId,
                    Quantity = item.Quantity,
                }),
            ],
            History = [
                .. domain.History.Select(item => new ServiceOrderHistoryEntity
                {
                    Id = item.Id,
                    ServiceOrderId = item.ServiceOrderId,
                    Status = item.Status,
                    CreatedAt = item.CreatedAt,
                    CreatedById = item.CreatedById,
                }),
            ],
        };
    }

    public static ServiceOrder ToDomain(this ServiceOrderEntity entity)
    {
        var domain = new ServiceOrder
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            VehicleId = entity.VehicleId,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            StartedAt = entity.StartedAt,
            FinishedAt = entity.FinishedAt,
        };

        domain.ReplaceServices(entity.Services.Select(item => item.ServiceId));
        domain.ReplaceSupplies(entity.Supplies.Select(item => (item.SupplyId, item.Quantity)));
        domain.LoadHistory(
            entity.History
                .OrderBy(item => item.CreatedAt)
                .Select(item =>
                    ServiceOrderHistory.Restore(
                        item.Id,
                        item.ServiceOrderId,
                        item.Status,
                        item.CreatedAt,
                        item.CreatedById
                    )
                )
        );

        return domain;
    }
}
