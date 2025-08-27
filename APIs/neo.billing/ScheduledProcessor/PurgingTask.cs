using neo.scheduler.Data.Enterprise;
using neo.scheduler.Facades;

namespace neo.scheduler.ScheduledProcessor
{
    public class PurgingTask : ScheduledProcessor
    {
        protected override TimeSpan InitialDelay => TimeSpan.FromSeconds(30); //delay initial execution by 30 seconds to ease Startup process
        protected override string Schedule => $"5 * * * *"; //every 5 minute

        public PurgingTask(IServiceScopeFactory serviceScopeFactory, CancellationToken ct) : base(serviceScopeFactory, ct)
        {

        }

        public async override Task ProcessInScope(IServiceProvider serviceProvider, CancellationToken ct)
        {
            try
            {
                var dbContext = serviceProvider.GetRequiredService<EnterpriseDbContext>();
                var purgingFacade = new PurgingFacade(dbContext);

                await purgingFacade.PurgeOldInactiveRecordsAsync(ct);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
