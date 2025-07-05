using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neo.admin.Data.Entities
{
    /// <summary>Maps to <b>sys_faskes</b> in db_neoclinic.</summary>
    [Table("sys_faskes")]
    public class Faskes
    {
        [Key] public long Id { get; set; }
        [Required, MaxLength(20)] public string NoFaskes { get; set; } = null!;
        [Required, MaxLength(255)] public string Name { get; set; } = null!;
        public long? CorporateId { get; set; }
        public Corporate Corporate { get; set; } = null!;

        [Required, MaxLength(255)] public string Email { get; set; } = null!;
        [Required, MaxLength(20)] public string PhoneNumber { get; set; } = null!;
        [Required] public string Address { get; set; } = null!;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime RegisteredDate { get; set; }
        public DateTime? InitPaymentDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public int? GracePeriod { get; set; }          // months
        public decimal? GracePenalty { get; set; }          // money

        public DateTime CreatedAt { get; set; }
        public long CreatorId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public long? UpdaterId { get; set; }

        /* nav */
        public ICollection<Login> Logins { get; set; } = new List<Login>();
    }
}
