using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Entities;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceOrderRepository(AppDbContext context) : IServiceOrderRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(ServiceOrder serviceOrder)
        {
            await _context.ServiceOrders.AddAsync(serviceOrder.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceOrder?> GetByIdAsync(Guid id)
        {
            var entity = await _context
                .ServiceOrders.Include(x => x.Services)
                .Include(x => x.Supplies)
                .Include(x => x.History.OrderBy(h => h.CreatedAt))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity?.ToDomain();
        }

        public async Task<List<ServiceOrder>> GetAllAsync(ServiceOrderStatus? status)
        {
            IQueryable<ServiceOrderEntity> query = _context
                .ServiceOrders.Include(x => x.Services)
                .Include(x => x.Supplies)
                .Include(x => x.History.OrderBy(h => h.CreatedAt))
                .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var entities = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return [.. entities.Select(x => x.ToDomain())];
        }

        public async Task UpdateAsync(ServiceOrder serviceOrder)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var exists = await _context.ServiceOrders.AnyAsync(x => x.Id == serviceOrder.Id);
            if (!exists)
            {
                throw new DbUpdateConcurrencyException(
                    $"Service order '{serviceOrder.Id}' was not found while updating."
                );
            }

            await _context
                .ServiceOrders.Where(x => x.Id == serviceOrder.Id)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(x => x.Status, serviceOrder.Status)
                        .SetProperty(x => x.StartedAt, serviceOrder.StartedAt)
                        .SetProperty(x => x.FinishedAt, serviceOrder.FinishedAt)
                );

            await _context
                .Set<ServiceOrderServiceEntity>()
                .Where(x => x.ServiceOrderId == serviceOrder.Id)
                .ExecuteDeleteAsync();

            if (serviceOrder.Services.Count != 0)
            {
                var serviceEntities = serviceOrder
                    .Services.Select(x => new ServiceOrderServiceEntity
                    {
                        ServiceOrderId = serviceOrder.Id,
                        ServiceId = x.ServiceId,
                    })
                    .ToList();

                await _context.Set<ServiceOrderServiceEntity>().AddRangeAsync(serviceEntities);
            }

            await _context
                .Set<ServiceOrderSupplyEntity>()
                .Where(x => x.ServiceOrderId == serviceOrder.Id)
                .ExecuteDeleteAsync();

            if (serviceOrder.Supplies.Count != 0)
            {
                var supplyEntities = serviceOrder
                    .Supplies.Select(x => new ServiceOrderSupplyEntity
                    {
                        ServiceOrderId = serviceOrder.Id,
                        SupplyId = x.SupplyId,
                        Quantity = x.Quantity,
                    })
                    .ToList();

                await _context.Set<ServiceOrderSupplyEntity>().AddRangeAsync(supplyEntities);
            }

            var existingHistoryIds = await _context
                .ServiceOrderHistories.Where(x => x.ServiceOrderId == serviceOrder.Id)
                .Select(x => x.Id)
                .ToListAsync();

            var existingHistorySet = existingHistoryIds.ToHashSet();
            var newHistoryEntities = serviceOrder
                .History.Where(x => !existingHistorySet.Contains(x.Id))
                .Select(x => new ServiceOrderHistoryEntity
                {
                    Id = x.Id,
                    ServiceOrderId = x.ServiceOrderId,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    CreatedById = x.CreatedById,
                })
                .ToList();

            if (newHistoryEntities.Count != 0)
            {
                await _context.ServiceOrderHistories.AddRangeAsync(newHistoryEntities);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<(
            int total,
            int completed,
            double averageHours,
            DateTime? earliest,
            DateTime? latest
        )> GetAverageExecutionTimeAsync()
        {
            var allOrders = await _context.ServiceOrders.ToListAsync();
            var completed = allOrders
                .Where(x =>
                    x.Status == ServiceOrderStatus.Finished
                    || x.Status == ServiceOrderStatus.Delivered
                )
                .ToList();

            var total = allOrders.Count;
            var completedCount = completed.Count;

            if (completedCount == 0)
            {
                return (total, 0, 0, null, null);
            }

            var executionTimes = completed
                .Where(x => x.StartedAt.HasValue && x.FinishedAt.HasValue)
                .Select(x => (x.FinishedAt!.Value - x.StartedAt!.Value).TotalHours)
                .ToList();

            var averageHours = executionTimes.Count != 0 ? executionTimes.Average() : 0;

            var earliest = completed.Min(x => x.StartedAt);
            var latest = completed.Max(x => x.FinishedAt);

            return (total, completedCount, averageHours, earliest, latest);
        }
    }
}
