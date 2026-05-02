using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();
            return Ok(result);
        }

        [HttpGet("recent-journal-entries")]
        public async Task<IActionResult> GetRecentJournalEntries([FromQuery] int take = 5)
        {
            var result = await _dashboardService.GetRecentJournalEntriesAsync(take);
            return Ok(result);
        }

    }
}
