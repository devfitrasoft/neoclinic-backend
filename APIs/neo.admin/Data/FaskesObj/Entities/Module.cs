using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_module</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_module")]
    public class Module
    {
        [Key] public int Id { get; set; }
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
