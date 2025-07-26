using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities.Objs.Enterprise
{

    /// <summary>
    /// Maps to <b>sys_billing</b> in db_neoclinic.
    /// </summary>
    [Table("sys_billing")]
    public class Billing
    {
        [Key, Column("id")] public long Id { get; set; }

        [Column("faskes_id")] public long FaskesId { get; set; }
        public Faskes Faskes { get; set; } = null!;

        [Column("period_start")] public DateTime PeriodStart { get; set; }  // e.g., Aug 1 (start of billing cycle)
        [Column("period_end")] public DateTime PeriodEnd { get; set; }      // e.g., Aug 31 23:59 (end of billing cycle)

        [Column("due_date")] public DateTime DueDate { get; set; }         // e.g., Sept 10 (max late payment before suspension)
        [Column("suspension_date")] public DateTime SuspensionDate { get; set; } // e.g., Sept 11 (beginning of suspension)
        [Column("grace_end_date")] public DateTime GraceEndDate { get; set; }   // e.g., Dec 11 (the date of faskes deletion)

        [Column("is_paid")] public bool IsPaid { get; set; } = false;    // flag for current row invoice
        [Column("payment_date")] public DateTime? PaymentDate { get; set; } // date of current invoice payment

        [Column("transaction_count")] public long TransactionCount { get; set; }    // total transaction during a period
        [Column("amount_due")] public decimal AmountDue { get; set; }   //  sys_billing_setting.transaction_price_per_unit x transaction_count

        [Column("grace_penalty")] public decimal GracePenalty { get; set; } //  grace penalty applied at the time (for audit)
        [Column("sum_grace_penalty")] public decimal? SumGracePenalty { get; set; } // sum of grace penalty each month starting from suspension date until payhment/grace end date

        [Column("is_soft_deleted")] public bool IsSoftDeleted { get; set; } = false;
    }
}
