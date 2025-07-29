using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class FaskesQueries
    {
        private readonly IEnterpriseDbContext _db;
        public FaskesQueries(IEnterpriseDbContext db) => _db = db;

        public Task<Faskes?> GetNotDeletedAsync(string noFaskes, CancellationToken ct) =>
            _db.Faskeses.Include(f => f.PICs)
                        .FirstOrDefaultAsync(f => f.NoFaskes == noFaskes
                                               && !f.IsDeleted, ct);

        public async Task<Faskes> AddNewAsync(RegisterFaskesRequest req, Corporate? corp, CancellationToken ct)
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
                NPWP = req.Npwp,
                Email = req.Email,
                Phone = req.Phone,
                Address = req.Address,
                CorporateId = corp?.Id,
                RegisteredDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 0
            };

            _db.Faskeses.Add(faskes);
            await _db.SaveChangesAsync(ct);

            return faskes;
        }

        public async Task<int> UpdateIsActiveAsync(Faskes faskes, bool isActive, CancellationToken ct)
        {
            faskes.IsActive = isActive;
            return await _db.SaveChangesAsync(ct);
        }
    }
}
