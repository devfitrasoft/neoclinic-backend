using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    /// <summary>
    /// Maps to <b>sys_otp</b> in db_neoclinic.
    /// Deleted periodically each days. (Reset user session)
    /// </summary>
    [Table("sys_auth_session")]
    public class AuthSession
    {
        [Key, Column("id")] public long Id { get; set; }
        [Required, Column("login_id")] public long LoginId { get; set; }
        
        [Required, Column("refresh_token_hash"), MaxLength(255)]
        public string RefreshTokenHash { get; set; } = null!;

        [Required, Column("issued_at")] public DateTime IssuedAt { get; set; }
        [Required, Column("expired_at")] public DateTime ExpiredAt { get; set; }
    }
}
