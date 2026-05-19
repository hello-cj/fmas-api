using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class VendorService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public VendorService(
            FMASDbContext context,
            CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<Vendor>> GetAllAsync()
        {
            return await _context.Vendors
                .Where(x => x.OrganizationId == _currentUser.OrganizationId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<Vendor?> GetByIdAsync(Guid id)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(x =>
                    x.VendorId == id &&
                    x.OrganizationId == _currentUser.OrganizationId);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<Guid> CreateAsync(CreateVendorDto dto)
        {
            var vendor = new Vendor
            {
                VendorId = Guid.NewGuid(),
                OrganizationId = _currentUser.OrganizationId!.Value,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            };

            _context.Vendors.Add(vendor);

            await _context.SaveChangesAsync();

            return vendor.VendorId;
        }

        // =========================
        // DELETE
        // =========================
        public async Task DeleteAsync(Guid id)
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(x =>
                    x.VendorId == id &&
                    x.OrganizationId == _currentUser.OrganizationId);

            if (vendor == null)
                throw new Exception("Vendor not found");

            _context.Vendors.Remove(vendor);

            await _context.SaveChangesAsync();
        }
    }
}