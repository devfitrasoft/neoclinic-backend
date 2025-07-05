using Microsoft.EntityFrameworkCore;
using neo.admin.Data;
using neo.admin.Models;

namespace neo.admin.Services
{
    public sealed class CorporateQueryService
    {
        private readonly EnterpriseDbContext _db;
        public CorporateQueryService(EnterpriseDbContext db) => _db = db;

        public Task<List<CorporateLookupItem>> SearchAsync(string term, CancellationToken ct) =>
            _db.Corporates
               .Where(c => !c.IsDeleted && EF.Functions.ILike(c.Name, $"%{term}%"))
               .Select(c => new CorporateLookupItem(c.Id, c.Name))
               .ToListAsync(ct);
    }
}
