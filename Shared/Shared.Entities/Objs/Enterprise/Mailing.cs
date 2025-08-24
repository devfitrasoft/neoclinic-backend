using Shared.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    [Table("sys_mailing")]
    public class Mailing
    {
        [Key, Column("id")] public long Id { get; set; }
        
        [Required, Column("recipient_mail"), MaxLength(255)]
        public string RecipientMail { get; set; } = null!;

        [Required, Column("subject"), MaxLength(500)]
        public string Subject { get; set; } = null!;

        [Required, Column("content", TypeName = "text")]
        public string Content { get; set; } = null!; // HTML body

        [Column("attachment_paths", TypeName = "text")]
        public string? AttachmentPaths { get; set; } // Comma-separated file paths or JSON

        [Column("status")]
        public MailingStatus Status { get; set; } = MailingStatus.Pending;

        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        [Column("generated_at")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Column("sent_at")]
        public DateTime? SentAt { get; set; }

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;
    }
}
