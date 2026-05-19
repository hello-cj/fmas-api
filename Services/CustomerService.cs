using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class CustomerService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public CustomerService(
            FMASDbContext context,
            CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<Customer>> GetAllAsync()
        {
            var orgId = _currentUser.OrganizationId!.Value;

            return await _context.Customers
                .Where(x => x.OrganizationId == orgId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var orgId = _currentUser.OrganizationId!.Value;

            return await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == id &&
                    x.OrganizationId == orgId);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<Guid> CreateAsync(CreateCustomerDto dto)
        {
            var orgId = _currentUser.OrganizationId!.Value;

            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                OrganizationId = orgId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return customer.CustomerId;
        }

        // =========================
        // DELETE
        // =========================
        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
                throw new Exception("Customer not found");

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }
    }
}