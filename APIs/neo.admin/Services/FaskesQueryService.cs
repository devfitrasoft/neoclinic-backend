// Services/FaskesQueryService.cs
using Microsoft.EntityFrameworkCore;
using neo.admin.Data;
using neo.admin.Models;

public sealed class FaskesQueryService
{
    private readonly EnterpriseDbContext _db;
    public FaskesQueryService(EnterpriseDbContext db) => _db = db;

    public Task<FaskesInfoResponse?> GetFaskesAsync(string noFaskes, CancellationToken ct) =>
        _db.Faskeses
           .Where(f => f.NoFaskes == noFaskes)
           .Select(f => new FaskesInfoResponse(
                f.Id, f.Name, f.Email, f.PhoneNumber, f.Address,
                f.IsActive, f.CorporateId,
                f.Corporate != null ? f.Corporate.Name : null))
           .FirstOrDefaultAsync(ct);
}

// Services/CorporateQueryService.cs
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
