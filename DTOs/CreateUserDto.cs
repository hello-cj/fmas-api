namespace FMAS.API.DTOs
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Accountant" & "Clerk"
    }
}
