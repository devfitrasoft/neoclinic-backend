using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class BillingSettingQueries
    {
        private readonly IBillingDbContext _db;
        public BillingSettingQueries(IBillingDbContext db) => _db = db;

        public async Task<BillingSetting?> GetLatestActiveSettingAsync(CancellationToken ct)
            => await _db.BillingSettings.OrderByDescending(r => r.CreatedAt)
                                         .FirstOrDefaultAsync(r => r.IsActive,ct);

        public async Task<BillingSetting> GetOrCreateActiveBillingSettingAsync(CancellationToken ct)
        {
            var active = await GetLatestActiveSettingAsync(ct);

            if (active != null)
                return active;

            // If no settings exist at all, insert default
            if (!await _db.BillingSettings.AnyAsync(ct))
            {
                var defaultSetting = new BillingSetting();

                _db.BillingSettings.Add(defaultSetting);
                await _db.SaveChangesAsync(ct);

                return defaultSetting;
            }

            throw new InvalidOperationException("No active billing setting found and cannot determine defaults.");
        }
    }
}
