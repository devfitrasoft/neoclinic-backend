using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Enterprise.Entities
{/// <summary>Maps to <b>sys_connstring</b> in db_neoclinic.</summary>
    [Table("sys_connstring")]
    public class ConnString
    {
        [Key] public long Id { get; set; }
        [Required] public long LoginId { get; set; }

        [MaxLength(255)] public string DbName { get; set; } = null!;
        [MaxLength(50)] public string DbHost { get; set; } = null!;
        [MaxLength(255)] public string DbUsername { get; set; } = null!;
        [MaxLength(255)] public string DbPassword { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

        public Login Login { get; set; } = null!;
    }
}
