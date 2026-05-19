namespace FMAS.API.DTOs.SuperAdmin
{
    public class ResetPasswordDto
    {
        public Guid OrganizationId { get; set; }

        public string NewPassword { get; set; }
    }
}