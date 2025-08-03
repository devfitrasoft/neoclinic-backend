using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class PICQueries
    {
        private readonly IPICDbContext _edb;

        public PICQueries(IPICDbContext edb) => _edb = edb;

        public async Task<PIC> AddAsync(long faskesId, string name, string email, string phone, PICCType picType, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var entity = new PIC
            {
                FaskesId = faskesId,
                Name = name,
                Email = email,
                Phone = phone,
                PICType = picType,
                CreatedAt = now
            };

            _edb.PICs.Add(entity);
            await _edb.SaveChangesAsync(ct);

            return entity;
        }

        public async Task<int> ActivateAsync(PIC pic, CancellationToken ct)
        {
            if (pic.IsActive && !pic.IsDeleted) return 1;

            pic.IsActive = true;
            pic.IsDeleted = false;
            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<List<PIC>> GetListByFaskesIdAsync(long faskesId, CancellationToken ct)
            => await _edb.PICs.Where(r => r.FaskesId == faskesId).ToListAsync(ct);

        public async Task<PIC?> GetByFaskesIdAndTypeAsync(long faskesId, PICCType type, CancellationToken ct)
            => await _edb.PICs.FirstOrDefaultAsync(r => r.FaskesId == faskesId && r.PICType == type, ct);
    }
}
