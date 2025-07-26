using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    /// <summary>Maps to <b>sys_faskes</b> in db_neoclinic.</summary>
    [Table("sys_faskes")]
    public class Faskes
    {
        [Key, Column("id")] public long Id { get; set; }

        [Required, MaxLength(20), Column("no_faskes")] 
        public string NoFaskes { get; set; } = null!;

        [Required, MaxLength(255), Column("name")] 
        public string Name { get; set; } = null!;
        
        [Required, MaxLength(20), Column("npwp")] 
        public string NPWP { get; set; } = null!;

        [Required, MaxLength(255), Column("email")]
        public string? Email { get; set; }

        [Required, MaxLength(20), Column("phone")] 
        public string Phone { get; set; } = null!;
        
        [Required, Column("address")] public string Address { get; set; } = null!;

        [Column("corporate_id")] public long? CorporateId { get; set; }
        public Corporate? Corporate { get; set; }

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;
        [Column("registered_date")] public DateTime RegisteredDate { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }

        /* nav */
        public ICollection<Login> Logins { get; set; } = new List<Login>();
        public ICollection<PIC> PICs { get; set; } = new List<PIC>();
        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
    }
}
