namespace FMAS.API.Controllers
{
    using FMAS.API.DTOs;
    using FMAS.API.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManagementService _service;

        public UsersController(UserManagementService service)
        {
            _service = service;
        }

        // CREATE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CreateUserDto dto)
        {
            _service.CreateUser(dto);
            return Ok("User created");
        }

        // READ
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetUsers());
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UpdateUserDto dto)
        {
            _service.UpdateUser(id, dto);
            return Ok("User updated");
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.DeleteUser(id);
            return Ok("User deleted");
        }
    }
}
