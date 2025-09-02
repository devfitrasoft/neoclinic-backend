using neo.scheduler.Data.Enterprise;
using neo.scheduler.Facades;

namespace neo.scheduler.ScheduledProcessor
{
    public class PurgingTask : ScheduledProcessor
    {
        readonly ILogger _logger;

        protected override TimeSpan InitialDelay => TimeSpan.FromSeconds(30); //delay initial execution by 30 seconds to ease Startup process
        protected override string Schedule => $"* * * * *"; //every 5 minute

        public PurgingTask(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory) : base(serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<PurgingTask>();
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
                _logger.LogError(ex, "PurgingTask exception");
            }
        }
    }
}
