using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Mailing;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Optional: wire up IEmailSender via IConfiguration (section defaults to "Smtp").
    /// </summary>
    public static IServiceCollection AddMailing(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "YourSectionName")
    {
        services.Configure<SmtpSettings>(configuration.GetSection(sectionName));
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
