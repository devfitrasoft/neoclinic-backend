using neo.admin.Data.Enterprise.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_user</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_user")]
    public class User
    {
        [Key] public long Id { get; set; }
        [Required] public long LoginId { get; set; }      // FK to db_neoclinic.sys_login
        public Login Login { get; set; } = null!;   // Except password_hash
        [Required] public int RoleId { get; set; }  // FK to db_neoclinic_{noFaskes}.sys_role
        public Role Role { get; set; } = null!;
        [MaxLength(50)] public string? NIK { get; set; }
        [Required, MaxLength(255)] public string Name { get; set; } = null!;

        [MaxLength(50)] public string? PCareUsername { get; set; }
        [MaxLength(255)] public string? PCarePasswordEnc { get; set; }
        [MaxLength(50)] public string? ICareUsername { get; set; }
        [MaxLength(255)] public string? ICarePasswordEnc { get; set; }
        [MaxLength(50)] public string? AntrolUsername { get; set; }
        [MaxLength(255)] public string? AntrolPasswordEnc { get; set; }
        [MaxLength(50)] public string? SSPracticionerId { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

    }
}
