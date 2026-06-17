using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Infrastructure.Data.Entities;

public class ServiceOrderHistoryEntity
{
    public Guid Id { get; set; }
    public Guid ServiceOrderId { get; set; }
    public ServiceOrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
}
