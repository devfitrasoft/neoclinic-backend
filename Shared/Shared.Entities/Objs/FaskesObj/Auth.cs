using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.FaskesObj
{
    /// <summary>Maps to <b>sys_auth</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_auth")]
    public class Auth
    {
        [Key, Column("id")] public int Id { get; set; }
        [Required, Column("role_id")] public int RoleId { get; set; }  // FK to db_neoclinic_{noFaskes}.sys_role
        public Role Role { get; set; } = null!;

        [Required, MaxLength(2), Column("module_code")] 
        public string ModuleCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_module
        public Module Module { get; set; } = null!;

        [Required, MaxLength(2), Column("group_code")] 
        public string GroupCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_group
        public Group Group { get; set; } = null!;

        [Required, MaxLength(2), Column("menu_code")] 
        public string MenuCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_menu
        public Menu Menu { get; set; } = null!;

        [Column("view")] public bool View { get; set; }
        [Column("add")] public bool Add { get; set; }
        [Column("edit")] public bool Edit { get; set; }
        [Column("delete")] public bool Delete { get; set; }
        [Column("print")] public bool Print { get; set; }

        [Column("is_active")] public bool? IsActive { get; set; }
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
