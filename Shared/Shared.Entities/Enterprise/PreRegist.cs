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

        [MaxLength(1), Required, Column("preferred_contact")] 
        public int PreferredContact { get; set; }
        
        [MaxLength(255), Column("email")] public string? Email { get; set; }
        [MaxLength(20), Column("phone")] public string? Phone { get; set; }
        
        [MaxLength(1), Required, Column("product_type")] 
        public int ProductType { get; set; }
        
        [MaxLength(255), Column("otp")] public string? Otp { get; set; }
        [Column("otp_expired_at")] public DateTime? OtpExpiresAt { get; set; }
        [Column("is_registered_web")] public bool IsRegisteredWeb { get; set; } = false;
        [Column("is_registered_desktop")] public bool IsRegisteredDesktop { get; set; } = false;
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}
