using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class BillingQueries
    {
        private IBillingDbContext _db;

        public BillingQueries(IBillingDbContext db) => _db = db;

        public async Task<Billing> GenerateRegistrationBillingAsync(long faskesId, BillingSetting setting, CancellationToken ct)
        {
            var generatedBilling = new Billing() {
                FaskesId = faskesId,
                BillingType = BillingType.Registration,
                AmountDue = setting.RegistrationFee,
            };

            _db.Billings.Add(generatedBilling);
            await _db.SaveChangesAsync(ct);

            return generatedBilling;
        }

        public async Task<int> MarkIsPaidTrueAsync(Billing billing, CancellationToken ct)
        {
            billing.IsPaid = true;
            return await _db.SaveChangesAsync(ct);
        }

        public async Task<Billing?> GetRegistrationBillingByFaskesIdAsync(long faskesId, CancellationToken ct)
            => await _db.Billings.FirstOrDefaultAsync(r => r.FaskesId == faskesId 
                                                        && r.BillingType == BillingType.Registration,ct);
    }
}
