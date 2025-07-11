using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_auth</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_auth")]
    public class Auth
    {
        [Key] public int Id { get; set; }
        [Required] public int RoleId { get; set; }  // FK to db_neoclinic_{noFaskes}.sys_role
        public Role Role { get; set; } = null!;

        [Required, MaxLength(2)] public string ModuleCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_module
        public Module Module { get; set; } = null!;
        [Required, MaxLength(2)] public string GroupCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_group
        public Group Group { get; set; } = null!;
        [Required, MaxLength(2)] public string MenuCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_menu
        public Menu Menu { get; set; } = null!;

        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Print { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }
    }
}
