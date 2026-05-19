namespace FMAS.API.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public string Action { get; set; } // CREATE / POST / UPDATE / DELETE
        
        public string Module { get; set; } //  // JournalEntry / ARInvoice / APInvoice

        public string EntityName { get; set; } // JournalEntry, ARInvoice, etc.

        public Guid? EntityId { get; set; }
        public string? UserEmail { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}