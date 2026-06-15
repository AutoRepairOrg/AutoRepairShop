using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Infrastructure.Data.Entities;

namespace AutoRepairShop.Infrastructure.Data.Mappings;

public static class PersistenceMappingExtensions
{
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
