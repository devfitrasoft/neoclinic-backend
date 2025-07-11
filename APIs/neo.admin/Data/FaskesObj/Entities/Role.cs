using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_role</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_role")]
    public class Role
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(50)] public string Name { get; set; } = null!;

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

        public ICollection<Auth> Auths { get; set; } = new List<Auth>();    // collection of authorizations a role has
    }
}
