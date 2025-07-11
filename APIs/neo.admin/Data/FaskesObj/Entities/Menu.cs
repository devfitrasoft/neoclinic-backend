using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_menu</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_menu")]
    public class Menu
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(2)] public string ModuleCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_module
        public Module Module { get; set; } = null!;
        [Required, MaxLength(2)] public string GroupCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_group
        public Group Group { get; set; } = null!;

        [Required, MaxLength(2)] public string Code { get; set; } = null!;
        [Required, MaxLength(50)] public string Name { get; set; } = null!;

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }
    }
}
