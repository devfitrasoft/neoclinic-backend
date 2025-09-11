using neo.scheduler.Data.Enterprise;
using Shared.Entities.Queries.Enterprise;

namespace neo.scheduler.Facades
{
    public interface IPurgingFacade
    {
        Task<int> PurgeOldInactiveRecordsAsync(CancellationToken ct);
    }

    public class PurgingFacade
    {
        private readonly OtpTokenQueries _otpQueries;

        public PurgingFacade(EnterpriseDbContext edb)
        {
            _otpQueries = new OtpTokenQueries(edb);
        }

        public async Task<int> PurgeOldInactiveRecordsAsync(CancellationToken ct)
        {
            int result = 0;

            result += await PurgeOldOtpAsync(ct); // removed used & expired OTPs (and reset AI counter if there are no records)

            return result;
        }

        private async Task<int> PurgeOldOtpAsync(CancellationToken ct)
        {
            var oldOtps = await _otpQueries.GetListOfUsedAndExpired(ct);

            if (oldOtps.Count() == 0)
                return 1;

            return await _otpQueries.PurgeUsedExpiryOtpAsync(ct);
        }
    }
}
