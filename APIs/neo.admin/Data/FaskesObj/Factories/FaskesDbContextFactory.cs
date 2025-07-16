using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Common;

namespace neo.admin.Data.FaskesObj.Factories
{
    /// <summary>
    /// Builds a FaskesDbContext for a specific clinic DB,
    /// and (optionally) creates that DB if it does not yet exist.
    /// </summary>
    public sealed class FaskesDbContextFactory
    {
        private readonly IConfiguration _cfg; 
        private readonly string _template;          // e.g. Host=postgres;Database={DB};Username=postgres;Password=admin
        private readonly string _adminConn;         // same host but to postgres database

        public FaskesDbContextFactory(IConfiguration cfg)
        {
            _cfg = cfg;
            _template = cfg.GetConnectionString("ClinicDBTemplate")
                  ?? throw new InvalidOperationException("ConnectionStrings:ClinicTemplate missing");
            _adminConn = cfg.GetConnectionString("ClinicAdmin")     // optional override
                       ?? _template.Replace("{DB}", "postgres");
        }

        /// <summary>
        /// Ensures database exists + migrations are up‑to‑date, then returns DbContext.
        /// </summary>
        public FaskesDbContext CreateAndMigrate(string noFaskes)
        {
            string dbName = $"db_neoclinic_{noFaskes}";
            var cstringDefaults = _cfg.GetSection("ClinicDbDefaults");
            EnsureDatabaseExists(dbName);

            var builder = new NpgsqlConnectionStringBuilder(_template)
            {
                Host = cstringDefaults.GetValue("Host", Constants.DB_FASKES_DEFAULT_HOST),
                Username = cstringDefaults.GetValue("Username", Constants.DB_FASKES_DEFAULT_USERNAME),
                Password = cstringDefaults.GetValue("Password", Constants.DB_FASKES_DEFAULT_PASSWORD),
                Database = dbName
            };
            string cs = builder.ToString();
            var opts = new DbContextOptionsBuilder<FaskesDbContext>()
                        .UseNpgsql(cs)
                        .Options;

            var ctx = new FaskesDbContext(opts);
            ctx.Database.Migrate();           // apply all migrations
            return ctx;
        }

        /* ---- private low‑level helper ---- */
        private void EnsureDatabaseExists(string dbName)
        {
            using var conn = new NpgsqlConnection(_adminConn);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT 1 FROM pg_database WHERE datname = @name;";
            cmd.Parameters.AddWithValue("name", dbName);

            var exists = cmd.ExecuteScalar() != null;
            if (exists) return;

            cmd.CommandText = @$"CREATE DATABASE ""{dbName}"" WITH ENCODING = 'UTF8';";
            cmd.Parameters.Clear();
            cmd.ExecuteNonQuery();
        }
    }
}
