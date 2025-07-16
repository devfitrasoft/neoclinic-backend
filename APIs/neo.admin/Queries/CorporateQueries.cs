using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Data.Enterprise.Entities;
using neo.admin.Models;

namespace neo.admin.Queries
{
    public sealed class CorporateQueries
    {
        private readonly EnterpriseDbContext _db;
        public CorporateQueries(EnterpriseDbContext db) => _db = db;

        public Task<List<CorporateLookupItem>> SearchAsync(string term, CancellationToken ct) =>
            _db.Corporates
               .Where(c => !c.IsDeleted && EF.Functions.ILike(c.Name, $"%{term}%"))
               .Select(c => new CorporateLookupItem(c.Id, c.Name))
               .ToListAsync(ct);

        public async Task<Corporate?> GetById(long id, CancellationToken ct) =>
            await _db.Corporates.FindAsync([id], ct);

        public async Task<Corporate> CreateCorporateIfMissing(string name, CancellationToken ct)
        {
            var upper = name.ToUpperInvariant();

            var corp = await _db.Corporates
                                .FirstOrDefaultAsync(c => c.Name == upper, ct);

            if (corp != null) return corp;

            corp = new Corporate
            {
                Name = upper,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 0
            };
            _db.Corporates.Add(corp);
            await _db.SaveChangesAsync(ct);
            return corp;
        }
    }
}
