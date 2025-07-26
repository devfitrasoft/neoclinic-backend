using Shared.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    [Table("sys_pic")]
    public class PIC
    {
        [Key, Column("id")] public long Id { get; set; }
        [Column("faskes_id")] public long FaskesId { get; set; }
        public Faskes Faskes { get; set; } = null!;

        [Required, MaxLength(255), Column("name")]
        public string Name { get; set; } = null!;

        [Required, MaxLength(255), Column("email")]
        public string Email { get; set; } = null!;

        [Required, MaxLength(20), Column("phone")]
        public string Phone { get; set; } = null!;

        [Required, Column("pic_type")]
        public PICCType PICType { get; set; }

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
