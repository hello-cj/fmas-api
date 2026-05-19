namespace FMAS.API.DTOs.SuperAdmin
{
    public class CreateOrganizationDto
    {
        public string OrganizationName { get; set; }

        public string AdminEmail { get; set; }

        public string AdminPassword { get; set; }
    }
}