using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Mailing;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// AddMailing(builder.Configuration) → registers SMTP sender + binds settings.
    /// </summary>
    public static IServiceCollection AddMailing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
