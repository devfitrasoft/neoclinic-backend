using Shared.Entities.Objs.Enterprise;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.FaskesObj
{
    /// <summary>Maps to <b>sys_user</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_user")]
    public class User
    {
        [Key, Column("id")] public long Id { get; set; }
        [Required, Column("login_id")] public long LoginId { get; set; }      // FK to db_neoclinic.sys_login
        public Login Login { get; set; } = null!;   // Except password_hash
        [Required, Column("role_id")] public int RoleId { get; set; }  // FK to db_neoclinic_{noFaskes}.sys_role
        public Role Role { get; set; } = null!;
        [MaxLength(50), Column("nik")] public string? NIK { get; set; }
        
        [Required, MaxLength(255), Column("name")] 
        public string Name { get; set; } = null!;

        [MaxLength(50), Column("pcare_username")] public string? PCareUsername { get; set; }
        [MaxLength(255), Column("pcare_password_enc")] public string? PCarePasswordEnc { get; set; }
        [MaxLength(50), Column("icare_username")] public string? ICareUsername { get; set; }
        [MaxLength(255), Column("icare_password_enc")] public string? ICarePasswordEnc { get; set; }
        [MaxLength(50), Column("antrol_username")] public string? AntrolUsername { get; set; }
        [MaxLength(255), Column("antrol_password_enc")] public string? AntrolPasswordEnc { get; set; }
        [MaxLength(50), Column("ss_practicioner_id")] public string? SSPracticionerId { get; set; }

        [Column("is_active")] public bool? IsActive { get; set; }
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }

    }
}
