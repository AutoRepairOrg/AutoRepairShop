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
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var persistedOrder = await _context
                    .ServiceOrders.Include(x => x.Services)
                    .Include(x => x.Supplies)
                    .Include(x => x.History)
                    .FirstOrDefaultAsync(x => x.Id == serviceOrder.Id);

                if (persistedOrder is null)
                {
                    throw new DbUpdateConcurrencyException(
                        $"Service order '{serviceOrder.Id}' was not found while updating."
                    );
                }

                persistedOrder.Status = serviceOrder.Status;
                persistedOrder.StartedAt = serviceOrder.StartedAt;
                persistedOrder.FinishedAt = serviceOrder.FinishedAt;

                persistedOrder.Services.Clear();
                foreach (var service in serviceOrder.Services)
                {
                    persistedOrder.Services.Add(
                        new ServiceOrderServiceEntity
                        {
                            ServiceOrderId = serviceOrder.Id,
                            ServiceId = service.ServiceId,
                        }
                    );
                }

                persistedOrder.Supplies.Clear();
                foreach (var supply in serviceOrder.Supplies)
                {
                    persistedOrder.Supplies.Add(
                        new ServiceOrderSupplyEntity
                        {
                            ServiceOrderId = serviceOrder.Id,
                            SupplyId = supply.SupplyId,
                            Quantity = supply.Quantity,
                        }
                    );
                }

                var existingHistoryIds = persistedOrder.History.Select(x => x.Id).ToHashSet();
                foreach (var history in serviceOrder.History.Where(x => !existingHistoryIds.Contains(x.Id)))
                {
                    persistedOrder.History.Add(
                        new ServiceOrderHistoryEntity
                        {
                            Id = history.Id,
                            ServiceOrderId = history.ServiceOrderId,
                            Status = history.Status,
                            CreatedAt = history.CreatedAt,
                            CreatedById = history.CreatedById,
                        }
                    );
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            });
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
