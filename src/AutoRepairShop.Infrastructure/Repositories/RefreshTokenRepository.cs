using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var entity = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            return entity?.ToDomain();
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            var persisted = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == token.Id);

            if (persisted is null)
                return;

            persisted.IsRevoked = token.IsRevoked;
            await _context.SaveChangesAsync();
        }
    }
}
