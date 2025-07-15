using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Enterprise.Entities
{
    /// <summary>Maps to <b>sys_login</b> in db_neoclinic.</summary>
    [Table("sys_login")]
    public class Login
    {
        [Key, Column("id")] public long Id { get; set; }

        [Required, MaxLength(255), Column("username")]
        public string Username { get; set; } = null!;   // format: <kode_faskes.username>

        [Required, MaxLength(255), Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Column("corporate_id")] public long? CorporateId { get; set; }
        public Corporate? Corporate { get; set; }

        [Column("faskes_id")] public long FaskesId { get; set; }
        public Faskes Faskes { get; set; } = null!;


        [Required, MaxLength(255), Column("email")]
        public string? Email { get; set; }

        [Required, MaxLength(20), Column("phone")]
        public string? PhoneNumber { get; set; }

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }
    }
}
