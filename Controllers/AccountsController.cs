using FMAS.API.Data;
using FMAS.API.DTOs.Accounts;
using FMAS.API.Services;
using FMAS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _service;
        private readonly FMASDbContext _context;

        public AccountsController(AccountService service, FMASDbContext context)
        {
            _service = service;
            _context = context;
        }

        // CREATE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CreateAccountDto dto)
        {
            _service.Create(dto);
            return Ok("Account created");
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        // UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Guid id, UpdateAccountDto dto)
        {
            _service.Update(id, dto);
            return Ok("Account updated");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdownAccounts()
        {
            var organizationId = User.FindFirst("organization_id")?.Value;

            if (organizationId == null)
                return Unauthorized();

            var accounts = await _context.Accounts
                .Where(a => a.OrganizationId == Guid.Parse(organizationId))
                .OrderBy(a => a.Code)
                .Select(a => new AccountDropdownDto
                {
                    Id = a.AccountId,
                    Code = a.Code,
                    Name = a.Name,
                    Type = a.AccountType.ToString()
                })
                .ToListAsync();

            return Ok(accounts);
        }

    }
}