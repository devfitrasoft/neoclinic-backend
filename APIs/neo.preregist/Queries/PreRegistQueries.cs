using Microsoft.EntityFrameworkCore;
using neo.admin.Common;
using neo.preregist.Data.Enterprise;
using neo.preregist.Data.Enterprise.Entities;
using neo.preregist.Models;
using Org.BouncyCastle.Ocsp;

namespace neo.preregist.Queries
{
    public sealed class PreRegistQueries
    {
        private readonly EnterpriseDbContext _db;
        public PreRegistQueries(EnterpriseDbContext db) => _db = db;

        public async Task<PreRegist?> GetRowByMailOrPhoneAsync(PreRegistRequest req, CancellationToken ct)
        {

            PrefComms prefComm = (PrefComms)req.PrefComm;
            ProductTypes product = (ProductTypes)req.ProductType;

            //  1. Check whether existing row exist or not based on the type of comms being passed by users
            var row = prefComm switch
            {
                PrefComms.Email => await _db.PreRegists.FirstOrDefaultAsync(r =>
                    r.Email != null && r.Email.ToLower() == req.Email.ToLower(), ct),

                PrefComms.Phone => await _db.PreRegists.FirstOrDefaultAsync(r =>
                    r.Phone != null && r.Phone == req.Phone, ct),

                PrefComms.Both => await _db.PreRegists.FirstOrDefaultAsync(r =>
                    r.Email != null && r.Email.ToLower() == req.Email.ToLower() &&
                    r.Phone != null && r.Phone == req.Phone, ct),

                _ => null
            };

            return row;
        }

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
                PreferredContact = req.PrefComm,
                ProductType = req.ProductType,
                Otp = newHashedOtp,
                OtpExpiresAt = newExpiresAt,
                IsRegisteredWeb = false,
                IsRegisteredDesktop = false,
                CreatedAt = now
            };

            _db.PreRegists.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateOtpAsync(PreRegist row, string hashedOtp, DateTime expiry, CancellationToken ct)
        {
            row.Otp = hashedOtp;
            row.OtpExpiresAt = expiry;
            row.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
        }
    }
}
