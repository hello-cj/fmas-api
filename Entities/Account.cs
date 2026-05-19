using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("accounts")]
    public class Account
    {
        public Guid AccountId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string AccountType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
