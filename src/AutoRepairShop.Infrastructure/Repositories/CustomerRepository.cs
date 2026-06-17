using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.ValueObjects;
using AutoRepairShop.Infrastructure.Data;
using AutoRepairShop.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer entity)
        {
            _context.Customers.Add(entity.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer entity)
        {
            var persisted = await _context.Customers.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            _context.Customers.Remove(persisted);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return [.. (await _context.Customers.ToListAsync()).Select(x => x.ToDomain())];
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            return entity?.ToDomain();
        }

        public async Task UpdateAsync(Customer entity)
        {
            var persisted = await _context.Customers.FirstOrDefaultAsync(c => c.Id == entity.Id);

            if (persisted is null)
                return;

            persisted.Name = entity.Name;
            persisted.Document = entity.Document.Value;
            persisted.Phone = entity.Phone;
            persisted.Username = entity.Username;
            persisted.Password = entity.Password;

            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByUserNameAsync(string username)
        {
            var entity = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
            return entity?.ToDomain();
        }

        public async Task<Customer?> GetByCpfCnpjAsync(string cpfCnpj)
        {
            var entity = await _context.Customers.FirstOrDefaultAsync(c => c.Document == cpfCnpj);
            return entity?.ToDomain();
        }
    }
}
