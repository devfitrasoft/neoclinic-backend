using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace Shared.Entities.Queries.Enterprise
{
    public class PreRegistQueries
    {
        private readonly ILogger _logger;
        private readonly IPreRegistDbContext _edb;

        public PreRegistQueries(ILoggerFactory loggerFactory, IPreRegistDbContext edb) 
        { 
            _edb = edb;
            _logger = loggerFactory.CreateLogger("PreRegistQueries");
        }

        public async Task<int> UpdatePreRegisteredFlagAsync(string regEmail, string regPhone, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(regEmail) && string.IsNullOrWhiteSpace(regPhone))
            {
                _logger.LogError("UpdatePreRegisteredFlagAsync: At least one of parameters must be filled");
                return 0;
            }

            var emailTrimmed = regEmail?.Trim().ToLower();
            var phoneTrimmed = regPhone?.Trim();

            var row = await _edb.PreRegists
                .FirstOrDefaultAsync(r =>
                    r.Email.ToLower().Contains(emailTrimmed) &&
                    r.Phone.Contains(phoneTrimmed), ct);

            if (row == null)
            {
                _logger.LogError("UpdatePreRegisteredFlagAsync: An entry could not be found for these particular parameter");
                return 0;
            }

            row.IsRegistered = true;
            row.UpdatedAt = DateTime.UtcNow;

            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<PreRegist?> GetRowByMailsync(PreRegistRequest req, CancellationToken ct)
            => await _edb.PreRegists.FirstOrDefaultAsync(r =>
                    r.Email != null && r.Email.ToLower() == req.Email.ToLower(), ct);

        public async Task<PreRegist> AddAsync(PreRegistRequest req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var entity = new PreRegist
            {
                Name = req.Name,
                Email = req.Email,
                Phone = req.Phone,
                IsRegistered = false,
                CreatedAt = now
            };

            _edb.PreRegists.Add(entity);
            await _edb.SaveChangesAsync(ct);

            return entity;
        }

        public async Task<int> UpdateInfoAsync(PreRegist row, PreRegistRequest req, CancellationToken ct)
        {
            row.Name = req.Name;
            row.Phone = req.Phone;
            row.UpdatedAt = DateTime.UtcNow;

            return await _edb.SaveChangesAsync(ct);
        }
    }
}
