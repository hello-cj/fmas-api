using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("user_roles")]

    public class UserRole
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("role_id")]
        public Guid RoleId { get; set; }
    }
}
