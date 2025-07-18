using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using Shared.Entities.Enterprise;

namespace neo.admin.Queries
{
    public class PreRegistQueries
    {
        private readonly ILogger<PreRegistQueries> _logger;
        private readonly EnterpriseDbContext _edb;

        public PreRegistQueries(ILogger<PreRegistQueries> logger, EnterpriseDbContext edb) 
        { 
            _edb = edb;
            _logger = logger;
        }

        public async Task UpdatePreRegisteredFlagAsync(string regEmail, string regPhone, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(regEmail) && string.IsNullOrWhiteSpace(regPhone))
            {
                _logger.LogError("UpdatePreRegisteredFlagAsync: At least one of parameters must be filled");
                return;
            }

            var row = await _edb.PreRegists.FirstOrDefaultAsync(r => r.Email.Contains(regEmail.Trim(), StringComparison.OrdinalIgnoreCase) && r.Phone.Contains(regPhone.Trim()), ct);

            if(row == null)
            {
                _logger.LogError("UpdatePreRegisteredFlagAsync: An entry could not be found for these particular parameter");
                return;
            }

            row.IsRegistered = true;
            row.UpdatedAt = DateTime.UtcNow;

            await _edb.SaveChangesAsync(ct);
        }
    }
}
