using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class OtpTokenQueries
    {
        private readonly IOtpTokenDbContext _edb;

        public OtpTokenQueries(IOtpTokenDbContext edb) =>_edb = edb;

        public async Task<IEnumerable<OtpToken>> GetListOfNotYetUsedByTargetId(long targetId, OtpType tokenType, CancellationToken ct)
            => await _edb.OtpTokens
                        .Where(r => r.Type == tokenType
                                 && r.TargetId == targetId
                                 && r.IsUsed == false
                        )
                        .ToListAsync(ct);

        public async Task<Tuple<PreRegist, DateTime>?> GetPreRegistAndExpiryByTokenAsync(string otp, CancellationToken ct)
        {
            var row = await _edb.OtpTokens
                               .FirstOrDefaultAsync(r => r.Code == otp
                                                      && r.Type == OtpType.PreRegist
                               , ct);

            if (row == null) return null;

            var preRegist = await _edb.PreRegists.FindAsync(new object[] { row.TargetId }, ct);

            if (preRegist == null) return null;

            return Tuple.Create((PreRegist)preRegist, (DateTime)row.ExpiredAt);
        }

        public async Task<int> MarkIsUsedAsync(string otp, OtpType tokenType, CancellationToken ct)
        {
            var row = await _edb.OtpTokens
                                .FirstOrDefaultAsync(r => r.Code == otp 
                                    && r.Type == tokenType
                                , ct);

            if(row == null) return 0;

            if(row.IsUsed) return 2;

            row.IsUsed = true;
            row.UpdatedAt = DateTime.UtcNow;
            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<bool> IsOtpUnused(string otp, OtpType tokenType, CancellationToken ct)
            => await _edb.OtpTokens
                         .AnyAsync(r => r.Code.Equals(otp)
                            && r.Type == tokenType
                            && r.IsUsed == false
                         , ct);

        public async Task<int> AddAsync(long targetId, string newHashedOtp, DateTime newExpiresAt, OtpType tokenType, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var entity = new OtpToken
            {
                TargetId = targetId,
                Code = newHashedOtp,
                Type = tokenType,
                ExpiredAt = newExpiresAt,
                CreatedAt = now
            };

            _edb.OtpTokens.Add(entity);
            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<int> RenewOtpAsync(OtpToken row, string newHashedOtp, DateTime newExpiresAt, CancellationToken ct)
        {
            row.Code = newHashedOtp;
            row.ExpiredAt = newExpiresAt;
            row.UpdatedAt = DateTime.UtcNow;

            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<long?> GetTargetIdByOtpAsync(string otp, OtpType tokenType, CancellationToken ct)
        {
            var token = await _edb.OtpTokens
                                  .FirstOrDefaultAsync(r => r.Code == otp && r.Type == tokenType, ct);

            if (token == null)
                return null;

            return token.TargetId;
        }
    }
}
