using neo.admin.Data.Enterprise.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.FaskesObj.Entities
{
    /// <summary>Maps to <b>sys_user_faskes</b> in db_neoclinic_{noFaskes}.</summary>
    [Table("sys_user_faskes")]
    public class UserFaskes
    {
        [Key] public int Id { get; set; }
        [Required] public long LoginId { get; set; }    // FK to db_neoclinic.sys_login
        [Required] public long FaskesId { get; set; }   // FK to db_neoclinic.sys_faskes

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

        [NotMapped] public Login Login { get; set; } = null!;
        [NotMapped] public Faskes Faskes { get; set; } = null!;
    }
}
