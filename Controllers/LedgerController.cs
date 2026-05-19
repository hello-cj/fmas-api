using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/ledger")]
    [Authorize]
    public class LedgerController : ControllerBase
    {
        private readonly LedgerService _service;

        public LedgerController(LedgerService service)
        {
            _service = service;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountLedger(
        Guid accountId,
        DateTime? from,
        DateTime? to)
            {
                var result = await _service.GetAccountLedgerAsync(
                    accountId,
                    from,
                    to);

                return Ok(result);
            }
        }
}