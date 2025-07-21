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

        [Column("corporate_id")] public long? CorporateId { get; set; }
        public Corporate Corporate { get; set; } = null!;

        [Required, MaxLength(255), Column("email")] 
        public string Email { get; set; } = null!;

        [Required, MaxLength(20), Column("phone")] 
        public string Phone { get; set; } = null!;

        [MaxLength(255), Column("email_bill")]
        public string? EmailBill { get; set; }

        [MaxLength(20), Column("phone_bill")]
        public string? PhoneBill { get; set; }

        [MaxLength(255), Column("email_tech")]
        public string? EmailTech { get; set; }

        [MaxLength(20), Column("phone_tech")]
        public string? PhoneTech { get; set; }

        [Required, Column("address")] public string Address { get; set; } = null!;

        [Column("is_active")] public bool IsActive { get; set; } = true;
        [Column("is_deleted")] public bool IsDeleted { get; set; } = false;

        [Column("registered_date")] public DateTime RegisteredDate { get; set; }
        [Column("init_payment_date")] public DateTime? InitPaymentDate { get; set; }
        [Column("last_payment_date")] public DateTime? LastPaymentDate { get; set; }
        [Column("expired_date")] public DateTime? ExpiredDate { get; set; }

        [Column("grace_period")] public int? GracePeriod { get; set; }          // months
        [Column("grace_penalty")] public decimal? GracePenalty { get; set; }          // money

        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("creator_id")] public long CreatorId { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
        [Column("updater_id")] public long? UpdaterId { get; set; }

        /* nav */
        public ICollection<Login> Logins { get; set; } = new List<Login>();
    }
}
