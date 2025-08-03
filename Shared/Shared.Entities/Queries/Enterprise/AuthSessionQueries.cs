using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public class AuthSessionQueries
    {
        private IEnterpriseDbContext _edb;
        
        public AuthSessionQueries(IEnterpriseDbContext edb) => _edb = edb;

        public async Task<AuthSession> AddSessionAsync(long loginId, string hashRefreshToken, string deviceId, string userAgent, CancellationToken ct)
        {
            var newSession = new AuthSession() {
                LoginId = loginId,
                RefreshTokenHash = hashRefreshToken,
                DeviceId = deviceId,
                UserAgent = userAgent,
                IssuedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            };

            _edb.AuthSessions.Add(newSession);
            await _edb.SaveChangesAsync(ct);
            return newSession;
        }

        public async Task<int> RefreshSessionAsync(AuthSession session, string hashRefreshToken, CancellationToken ct)
        {
            session.RefreshTokenHash = hashRefreshToken;
            session.IssuedAt = DateTime.UtcNow;
            session.LastActiveAt = DateTime.UtcNow;

            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<AuthSession?> GetSessionRefreshTokenAsync(string hashRefreshToken, string deviceId, string userAgent, CancellationToken ct)
            => await _edb.AuthSessions.FirstOrDefaultAsync(r => r.RefreshTokenHash == hashRefreshToken
                                                             && r.DeviceId == deviceId
                                                             && r.UserAgent == userAgent, ct);

        public async Task<int> InvalidateSpecificTokenSessionAsync(long loginId, string hashRefreshToken, string deviceId, string userAgent, CancellationToken ct)
        {
            var session = await _edb.AuthSessions
                .FirstOrDefaultAsync(s => s.LoginId == loginId 
                                       && s.RefreshTokenHash == hashRefreshToken
                                       && s.DeviceId == deviceId
                                       && s.UserAgent == userAgent, ct);

            if (session != null)
            {
                _edb.AuthSessions.Remove(session);
                return await _edb.SaveChangesAsync(ct);
            }

            return 0;
        }

        public async Task<int> InvalidateAllSessionsByLoginIdPerDevice(long loginId, string deviceId, string userAgent, CancellationToken ct)
        {

            var session = await _edb.AuthSessions
                .FirstOrDefaultAsync(s => s.LoginId == loginId
                                       && s.DeviceId == deviceId
                                       && s.UserAgent == userAgent, ct);

            if (session != null)
            {
                _edb.AuthSessions.Remove(session);
                return await _edb.SaveChangesAsync(ct);
            }

            return 0;
        }

        public async Task<int> InvalidateAllSessionsAsync(CancellationToken ct)
        {
            var sessions = await _edb.AuthSessions.ToListAsync();

            if (sessions.Any())
            {
                _edb.AuthSessions.RemoveRange(sessions);
                return await _edb.SaveChangesAsync(ct);
            }

            return 0;
        }

        public async Task ResetAuthSessionsAsync(CancellationToken ct)
            => await _edb.ExecuteRawSqlAsync("TRUNCATE TABLE sys_auth_session RESTART IDENTITY", ct);
    }
}
