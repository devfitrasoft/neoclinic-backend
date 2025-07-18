using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Models;
using Shared.Entities.Enterprise;

namespace neo.admin.Queries
{
    public sealed class FaskesQueries
    {
        private readonly EnterpriseDbContext _db;
        public FaskesQueries(EnterpriseDbContext db) => _db = db;

        public Task<Faskes?> GetAsync(string noFaskes, CancellationToken ct) =>
            _db.Faskeses.FirstOrDefaultAsync(f => f.NoFaskes == noFaskes, ct);

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
                Phone = req.Phone,
                EmailBill = req.EmailBill,
                PhoneBill = req.PhoneBill,
                EmailTech = req.EmailTech,
                PhoneTech = req.PhoneTech,
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
