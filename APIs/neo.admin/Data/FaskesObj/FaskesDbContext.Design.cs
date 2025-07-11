using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace neo.admin.Data.FaskesObj
{
    public sealed class FaskesDbContextDesignFactory
       : IDesignTimeDbContextFactory<FaskesDbContext>
    {
        public FaskesDbContext CreateDbContext(string[] args)
        {
            // Any valid connection string is fine *at design time*.
            // We point at the enterprise DB just so EF can query the server’s metadata.
            var cfg = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .Build();

            var cs = cfg.GetConnectionString("EnterpriseDB");

            var opts = new DbContextOptionsBuilder<FaskesDbContext>()
                       .UseNpgsql(cs)
                       .Options;

            return new FaskesDbContext(opts);
        }
    }
}
