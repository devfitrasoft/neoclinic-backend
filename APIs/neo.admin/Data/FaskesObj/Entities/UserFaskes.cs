using neo.admin.Data.Enterprise.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_user_faskes</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_user_faskes")]
    public class UserFaskes
    {
        [Key, Column("id")] public int Id { get; set; }
        [Required, Column("login_id")] public long LoginId { get; set; }    // FK to db_neoclinic.sys_login
        [NotMapped] public Login Login { get; set; } = null!;

        [Required, Column("faskes_id")] public long FaskesId { get; set; }   // FK to db_neoclinic.sys_faskes
        [NotMapped] public Faskes Faskes { get; set; } = null!;

        [Column("is_active")] public bool? IsActive { get; set; }
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
