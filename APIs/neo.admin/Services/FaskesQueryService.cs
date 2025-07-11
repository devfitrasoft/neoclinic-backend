// Services/FaskesQueryService.cs
using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Data.Enterprise.Entities;
using neo.admin.Models;

namespace neo.admin.Data.Services
{
    public sealed class FaskesQueryService
    {
        private readonly EnterpriseDbContext _db;
        public FaskesQueryService(EnterpriseDbContext db) => _db = db;

        public Task<FaskesInfoResponse?> GetAsync(string noFaskes, CancellationToken ct) =>
            _db.Faskeses
               .Where(f => f.NoFaskes == noFaskes)
               .Select(f => new FaskesInfoResponse(
                    f.Id, f.Name, f.Email, f.PhoneNumber, f.Address,
                    f.IsActive, f.CorporateId,
                    f.Corporate != null ? f.Corporate.Name : null))
               .FirstOrDefaultAsync(ct);

        public async Task<Faskes> CreateNewFaskes(RegisterFaskesRequest req, Corporate? corp, CancellationToken ct)
        {
            // Double-check again to avoid race condition
            var existing = await _db.Faskeses
                                    .FirstOrDefaultAsync(f => f.NoFaskes == req.NoFaskes, ct);
            if (existing != null)
                return existing;

            var faskes = new Faskes
            {
                NoFaskes = req.NoFaskes,
                Name = req.Name,
                CorporateId = corp?.Id,
                Corporate = corp,
                Email = req.Email,
                PhoneNumber = req.Phone,
                Address = req.Address,
                RegisteredDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 0
            };

            _db.Faskeses.Add(faskes);
            await _db.SaveChangesAsync(ct);

            return faskes;
        }
    }
}