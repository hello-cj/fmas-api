namespace FMAS.API.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("organizations")]
    public class Organization
    {
        [Key]
        [Column("organization_id")]
        public Guid OrganizationId { get; set; }


        [Column("name")]
        public string Name { get; set; }


        [Column("email")]
        public string Email { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
