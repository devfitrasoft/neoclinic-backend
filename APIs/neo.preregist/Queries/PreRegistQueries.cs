using Microsoft.EntityFrameworkCore;
using neo.preregist.Data.Enterprise;
using neo.preregist.Models;
using Shared.Entities.Enterprise;

namespace neo.preregist.Queries
{
    public sealed class PreRegistQueries
    {
        private readonly EnterpriseDbContext _db;
        public PreRegistQueries(EnterpriseDbContext db) => _db = db;

        public async Task<PreRegist?> GetRowByMailsync(PreRegistRequest req, CancellationToken ct)
            => await _db.PreRegists.FirstOrDefaultAsync(r =>
                    r.Email != null && r.Email.ToLower() == req.Email.ToLower(), ct);

        public async Task<PreRegist?> GetRowByTokenAsync(string token,  CancellationToken ct)
            => await _db.PreRegists.FirstOrDefaultAsync(r => r.Otp == token);

        public async Task AddAsync(PreRegistRequest req, string? newHashedOtp, DateTime? newExpiresAt, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var entity = new PreRegist
            {
                Name = req.Name,
                Email = req.Email,
                Phone = req.Phone,
                Otp = newHashedOtp,
                OtpExpiresAt = newExpiresAt,
                IsRegistered = false,
                CreatedAt = now
            };

            _db.PreRegists.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateInfoAndOtpAsync(PreRegist row, string hashedOtp, DateTime expiry, PreRegistRequest req, CancellationToken ct)
        {
            row.Name = req.Name;
            row.Phone = req.Phone;
            row.Otp = hashedOtp;
            row.OtpExpiresAt = expiry;
            row.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
        }
    }
}
