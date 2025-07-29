using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{
    [Table("sys_billing_setting")]
    public class BillingSetting
    {
        [Key, Column("id")] public int Id { get; set; }

        [Column("default_grace_period_months")] public int GracePeriodMonths { get; set; } = 3;
        [Column("default_grace_penalty")] public decimal GracePenalty { get; set; } = 6000000;
        [Column("registration_fee")] public decimal RegistrationFee { get; set; } = 6000000;
        [Column("transaction_price_per_unit")] public decimal TransactionPricePerUnit { get; set; } = 5000;
        [Column("is_active")] public bool IsActive { get; set; } = true;    // use active rule
        [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // for versioning rule
    }
}
