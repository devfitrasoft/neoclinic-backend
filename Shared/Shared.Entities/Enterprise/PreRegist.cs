using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Enterprise
{
    /// <summary>Maps to <b>pre_regist</b> in db_neoclinic.</summary>
    [Table("pre_regist")]
    public class PreRegist
    {
        [Key, Column("id")] public long Id { get; set; }

        [MaxLength(255), Required, Column("name")]
        public string Name { get; set; } = null!;
        
        [MaxLength(255), Required, Column("email")] 
        public string Email { get; set; } = null!;

        [MaxLength(20), Required, Column("phone")]
        public string Phone { get; set; } = null!;
        
        [MaxLength(255), Column("otp")] public string? Otp { get; set; }
        [Column("otp_expired_at")] public DateTime? OtpExpiresAt { get; set; }
        [Column("is_registered")] public bool IsRegistered { get; set; } = false;
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}
