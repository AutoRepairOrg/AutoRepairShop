using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Infrastructure.Data.Entities;

public class ServiceOrderEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    public ServiceOrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public List<ServiceOrderServiceEntity> Services { get; set; } = [];
    public List<ServiceOrderSupplyEntity> Supplies { get; set; } = [];
    public List<ServiceOrderHistoryEntity> History { get; set; } = [];
}
