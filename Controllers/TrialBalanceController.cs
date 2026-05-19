using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/trial-balance")]
    [Authorize]
    public class TrialBalanceController : ControllerBase
    {
        private readonly TrialBalanceService _service;

        public TrialBalanceController(TrialBalanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetTrialBalance());
        }
    }
}