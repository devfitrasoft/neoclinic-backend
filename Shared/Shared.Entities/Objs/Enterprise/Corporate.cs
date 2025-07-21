using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    /// <summary>Maps to <b>sys_corporate</b> in db_neoclinic.</summary>
    [Table("sys_corporate")]
    public class Corporate
    {
        [Key, Column("id")] public long Id { get; set; }

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;     // store upper‑case in code or via trigger

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }

        /* nav */
        public ICollection<Faskes> Faskes { get; set; } = new List<Faskes>();
    }
}
