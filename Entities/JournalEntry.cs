namespace FMAS.API.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("journal_entries")]
    public class JournalEntry
    {
        [Key]
        [Column("journal_entry_id")]
        public Guid JournalEntryId { get; set; }


        [Column("organization_id")]
        public Guid OrganizationId { get; set; }


        [Column("date")]
        public DateTime Date { get; set; }


        [Column("reference")]
        public string Reference { get; set; }


        [Column("description")]
        public string Description { get; set; }


        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        public List<JournalEntryLine> Lines { get; set; } = new();
    }
}
