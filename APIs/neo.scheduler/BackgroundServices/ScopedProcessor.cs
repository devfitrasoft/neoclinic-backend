namespace neo.scheduler.BackgroundServices
{
    public abstract class ScopedProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedProcessor(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task Process(CancellationToken ct)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider, ct);
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider, CancellationToken ct);
    }
}
