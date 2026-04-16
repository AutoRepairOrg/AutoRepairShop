using AutoRepairShop.Domain.Entities;

namespace AutoRepairShop.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task SaveAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
    }
}
