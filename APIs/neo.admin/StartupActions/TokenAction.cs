using neo.admin.Data.Enterprise;
using neo.admin.Facades;
using neo.admin.Services.Token;

namespace neo.admin.StartupActions
{
    public class TokenAction : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using var scope = app.ApplicationServices.CreateScope();

                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                var edb = scope.ServiceProvider.GetRequiredService<EnterpriseDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginFacade>>();

                var loginFacade = new LoginFacade(logger, tokenService, edb);
                loginFacade.LogoutAllAsync(CancellationToken.None).GetAwaiter().GetResult(); // sync wait

                next(app);
            };
        }
    }
}
