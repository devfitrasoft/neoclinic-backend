using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Enterprise.Entities
{
    /// <summary>Maps to <b>sys_connstring</b> in db_neoclinic.</summary>
    [Table("sys_connstring")]
    public class ConnString
    {
        [Key, Column("id")] public long Id { get; set; }
        [Required, Column("login_id")] public long LoginId { get; set; }
        public Login Login { get; set; } = null!;

        [MaxLength(255), Column("db_name")] public string DbName { get; set; } = null!;
        [MaxLength(50), Column("db_host")] public string DbHost { get; set; } = null!;
        [MaxLength(255), Column("db_username")] public string DbUsername { get; set; } = null!;
        [MaxLength(255), Column("db_password")] public string DbPassword { get; set; } = null!;

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
