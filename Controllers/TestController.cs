namespace FMAS.API.Controllers
{
    using FMAS.API.Data;
    using Microsoft.AspNetCore.Mvc;
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly FMASDbContext _context;

        public TestController(FMASDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = _context.Organizations.ToList();
            return Ok(data);
        }
    }
}
