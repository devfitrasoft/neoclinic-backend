using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Entities
{
    /// <summary>Maps to <b>sys_login</b> in db_neoclinic.</summary>
    [Table("sys_login")]
    public class Login
    {
        [Key] public long Id { get; set; }

        [Required, MaxLength(255)]
        public string Username { get; set; } = null!;   // format: <kode_faskes.username>

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        public long? CorporateId { get; set; }
        public Corporate? Corporate { get; set; }

        public long FaskesId { get; set; }
        public Faskes Faskes { get; set; } = null!;

        [MaxLength(255)] public string? Email { get; set; }
        [MaxLength(20)] public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }
    }
}
