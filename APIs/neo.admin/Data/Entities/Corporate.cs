using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Entities
{
    /// <summary>Maps to <b>sys_corporate</b> in db_neoclinic.</summary>
    [Table("sys_corporate")]
    public class Corporate
    {
        [Key] public long Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;     // store upper‑case in code or via trigger

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

        /* nav */
        public ICollection<Faskes> Faskes { get; set; } = new List<Faskes>();
    }
}
