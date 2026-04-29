using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("journal_entry_lines")]
    public class JournalEntryLine
    {
        [Key]
        [Column("line_id")]
        public Guid LineId { get; set; }


        [Column("journal_entry_id")]
        public Guid JournalEntryId { get; set; }


        [Column("account_id")]
        public Guid AccountId { get; set; }


        [Column("debit")]
        public decimal Debit { get; set; }


        [Column("credit")]
        public decimal Credit { get; set; }
    }
}
