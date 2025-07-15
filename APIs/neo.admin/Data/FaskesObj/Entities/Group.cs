using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_group</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_group")]
    public class Group
    {
        [Key, Column("id")] public int Id { get; set; }
        
        [Required, MaxLength(2), Column("module_code")] 
        public string ModuleCode { get; set; } = null!; // FK to db_neoclinic_{noFaskes}.sys_module
        public Module Module { get; set; } = null!;
        
        [Required, MaxLength(2), Column("code")] 
        public string Code { get; set; } = null!;

        [Required, MaxLength(50), Column("name")] 
        public string Name { get; set; } = null!;

        [Column("is_active")] public bool? IsActive { get; set; }
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
