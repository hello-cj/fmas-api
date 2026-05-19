namespace FMAS.API.Entities
{
    public class Vendor
    {
        public Guid VendorId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}