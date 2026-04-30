using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class ServiceOrderRepository(AppDbContext context) : IServiceOrderRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(ServiceOrder serviceOrder)
        {
            await _context.ServiceOrders.AddAsync(serviceOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceOrder?> GetByIdAsync(Guid id)
        {
            return await _context
                .ServiceOrders.Include(x => x.Services)
                .Include(x => x.Supplies)
                .Include(x => x.History.OrderBy(h => h.CreatedAt))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ServiceOrder>> GetAllAsync(ServiceOrderStatus? status)
        {
            IQueryable<ServiceOrder> query = _context
                .ServiceOrders.Include(x => x.Services)
                .Include(x => x.Supplies)
                .Include(x => x.History.OrderBy(h => h.CreatedAt));

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task UpdateAsync(ServiceOrder serviceOrder)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var latestHistory = serviceOrder
                    .History.OrderByDescending(h => h.CreatedAt)
                    .FirstOrDefault();

                if (latestHistory is not null)
                {
                    var historyEntry = _context.Entry(latestHistory);

                    if (historyEntry.State == EntityState.Detached)
                    {
                        await _context.ServiceOrderHistories.AddAsync(latestHistory);
                    }

                    await _context.SaveChangesAsync();
                }

                var affectedRows = await _context
                    .ServiceOrders.Where(x => x.Id == serviceOrder.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(x => x.Status, serviceOrder.Status)
                            .SetProperty(x => x.StartedAt, serviceOrder.StartedAt)
                            .SetProperty(x => x.FinishedAt, serviceOrder.FinishedAt)
                    );

                if (affectedRows == 0)
                {
                    throw new DbUpdateConcurrencyException(
                        $"Service order '{serviceOrder.Id}' was not found while updating."
                    );
                }

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
