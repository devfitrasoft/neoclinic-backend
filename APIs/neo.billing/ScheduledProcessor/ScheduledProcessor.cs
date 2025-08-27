using NCrontab;
using neo.scheduler.BackgroundServices;

namespace neo.scheduler.ScheduledProcessor
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        protected CrontabSchedule schedule;
        private DateTime _nextRun;
        protected virtual TimeSpan InitialDelay => TimeSpan.Zero;
        protected abstract string Schedule { get; }
        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, CancellationToken ct) : base(serviceScopeFactory)
        {
            schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = schedule.GetNextOccurrence(DateTime.Now);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (InitialDelay > TimeSpan.Zero)
            {
                // set one time delay, for the Startup
                await Task.Delay(InitialDelay, stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var delay = _nextRun - now; // calculating next delay dynamically until next scheduled run to avoids constant polling every 5 seconds, reducing CPU usage and contention.
                var nextrun = schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await Process(stoppingToken);
                    _nextRun = schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
