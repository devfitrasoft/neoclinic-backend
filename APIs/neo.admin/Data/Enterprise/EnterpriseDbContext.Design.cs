using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace neo.admin.Data.Enterprise
{
    public class EnterpriseDbContextDesignFactory : IDesignTimeDbContextFactory<EnterpriseDbContext>
    {
        public EnterpriseDbContext CreateDbContext(string[] args)
        {
            // 1. read base config (no need to build the full host)
            var cfg = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .Build();

            // 2. plain connection string from config
            string cs = cfg.GetConnectionString("EnterpriseDB")
                       ?? throw new InvalidOperationException("EnterpriseDB cs missing");

            // 3. build options
            var opts = new DbContextOptionsBuilder<EnterpriseDbContext>()
                       .UseNpgsql(cs)
                       .Options;

            return new EnterpriseDbContext(opts);
        }
    }
}
