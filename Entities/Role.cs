using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FMAS.API.Entities
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public Guid RoleId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}
