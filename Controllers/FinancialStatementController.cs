using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/financial-statements")]
    [Authorize]
    public class FinancialStatementsController : ControllerBase
    {
        private readonly FinancialStatementService _service;

        public FinancialStatementsController(
            FinancialStatementService service)
        {
            _service = service;
        }

        [HttpGet("income-statement")]
        public IActionResult GetIncomeStatement()
        {
            return Ok(_service.GetIncomeStatement());
        }

        [HttpGet("balance-sheet")]
        public IActionResult GetBalanceSheet()
        {
            return Ok(_service.GetBalanceSheet());
        }
    }
}