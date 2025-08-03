using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class CorporateQueries
    {
        private readonly IEnterpriseDbContext _db;
        public CorporateQueries(IEnterpriseDbContext db) => _db = db;

        public Task<List<CorporateLookupItemModel>> SearchNotDeletedAsync(string term, CancellationToken ct) =>
            _db.Corporates
               .Where(c => !c.IsDeleted && EF.Functions.ILike(c.Name, $"%{term}%"))
               .Select(c => new CorporateLookupItemModel(c.Id, c.Name))
               .ToListAsync(ct);

        public async Task<Corporate?> GetByIdAsync(long id, CancellationToken ct) =>
            await _db.Corporates.FindAsync([id], ct);

        public async Task<Corporate> CreateCorporateIfMissing(string name, string nameOwner, string mailOwner, string phoneOwner, CancellationToken ct)
        {
            var upper = name.ToUpperInvariant();

            var corp = await _db.Corporates
                                .FirstOrDefaultAsync(c => c.Name == upper, ct);

            if (corp != null) return corp;

            corp = new Corporate
            {
                Name = upper,
                NameOwner = nameOwner,
                EmailOwner = mailOwner,
                PhoneOwner = phoneOwner,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 0
            };
            _db.Corporates.Add(corp);
            await _db.SaveChangesAsync(ct);
            return corp;
        }

        public async Task<int> ActivateAsync(Corporate corporate, CancellationToken ct)
        {
            if (corporate.IsActive && !corporate.IsDeleted) return 1;

            corporate.IsActive = true;
            corporate.IsDeleted = false;
            return await _db.SaveChangesAsync(ct);
        }
    }
}
