using FMAS.API.Data;
using FMAS.API.DTOs.Accounts;
using FMAS.API.Entities;

namespace FMAS.API.Services
{
    public class AccountService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public AccountService(
            FMASDbContext context,
            CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // CREATE ACCOUNT
        public void Create(CreateAccountDto dto)
        {
            if (!_currentUser.OrganizationId.HasValue)
                throw new Exception("Organization not found");

            var orgId = _currentUser.OrganizationId.Value;

            var exists = _context.Accounts.Any(a =>
                a.Code == dto.Code &&
                a.OrganizationId == orgId);

            if (exists)
                throw new Exception("Account code already exists");

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                OrganizationId = orgId,
                Code = dto.Code,
                Name = dto.Name,
                AccountType = dto.AccountType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        // GET ALL
        public List<object> GetAll()
        {
            var orgId = _currentUser.OrganizationId;

            return _context.Accounts
                .Where(a => a.OrganizationId == orgId)
                .OrderBy(a => a.Code)
                .Select(a => new
                {
                    a.AccountId,
                    a.Code,
                    a.Name,
                    a.AccountType,
                    a.IsActive
                })
                .ToList<object>();
        }

        // UPDATE
        public void Update(Guid id, UpdateAccountDto dto)
        {
            var orgId = _currentUser.OrganizationId;

            var account = _context.Accounts
                .FirstOrDefault(a =>
                    a.AccountId == id &&
                    a.OrganizationId == orgId);

            if (account == null)
                throw new Exception("Account not found");

            account.Name = dto.Name;
            account.IsActive = dto.IsActive;

            _context.SaveChanges();
        }
    }
}