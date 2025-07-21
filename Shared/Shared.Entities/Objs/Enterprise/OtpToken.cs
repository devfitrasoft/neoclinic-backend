using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    /// <summary>
    /// Maps to <b>sys_otp</b> in db_neoclinic.
    /// Deleted periodically each days.
    /// </summary>
    [Table("sys_otp")]
    public sealed class OtpToken
    {
        [Key, Column("id")] public long Id { get; set; }
        [Required, Column("target_id")] public long TargetId { get; set; } // ID of PreRegist or Login, depending on Type

        [Required, Column("code"), MaxLength(255)]
        public string Code { get; set; } = null!;

        [Required, Column("type")] public int Type { get; set; }
        [Column("is_used")] public bool IsUsed { get; set; } = false;
        [Required, Column("expired_at")] public DateTime ExpiredAt { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }


        // Optional navigation helpers (not mapped, lazy loaded manually)
        [NotMapped] public PreRegist? PreRegist { get; set; }
        [NotMapped] public Login? Login { get; set; }

    }
}
