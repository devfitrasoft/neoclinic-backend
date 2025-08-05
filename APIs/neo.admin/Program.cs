using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Models;
using neo.admin.Services;
using neo.admin.Services.Factories;
using neo.admin.Services.Token;
using neo.admin.StartupActions;
using Shared.Common;
using Shared.Communication.DependencyInjection;
using Shared.EFCore;
using Shared.Entities.Queries;
using Shared.Logging;
using Shared.Mailing;


var b = WebApplication.CreateBuilder(args);

/* Registration configs */
b.Services.Configure<RegistrationSettingsModel>(
    b.Configuration.GetSection("Registration"));

/* using Shared.Logging library */
LoggingSetup.ConfigureBootstrapLogger(b.Configuration);
b.Host.UseSerilogLogging(b.Configuration);

// add worker only when file sink active
if (b.Configuration.GetValue("Logging:EnableFileSink", true))
    b.Services.AddHostedService<LogMaintenanceWorker>();  // register background compressor

/*  Load DBContexts */
// Register the DbContext
b.Services.AddDbContext<EnterpriseDbContext>(o =>
    o.UseNpgsql(b.Configuration.GetConnectionString("EnterpriseDB")));

// Register each interface mapping to the same instance
b.Services.AddScoped<IPICDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IBillingDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IOtpTokenDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IPreRegistDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IEnterpriseDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("sys_billing_setting", "sys_corporate", 
    "sys_faskes", "sys_billing", "sys_login", "sys_auth_session", "sys_pic");
/* --------------------------------------------- */

/*  Load base libraries */
b.Services.AddSharedRestClient();         // registers RestClient
b.Services.AddMailing(b.Configuration);   // SMTP

/*  Load token-related services */
b.Services.AddScoped<IJwtProvider, JwtProvider>();
b.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
b.Services.AddScoped<ITokenService, TokenService>();

/*  Load services   */
b.Services.AddScoped<MailService>();
b.Services.AddScoped<ICaptchaValidatorService, CaptchaValidatorService>();

/*  Set Cors Policy for UI    */
b.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(b.Configuration.GetValue<string>("App:RegisterWebUrl") ?? Constants.REGISTER_FRONT_END_URL)
              .AllowAnyHeader()
              .AllowCredentials()
              .AllowAnyMethod());
});

b.Services.AddSingleton<IStartupFilter, TokenAction>();

b.Services.AddControllers();

var app = b.Build();

app.UseCors();   // uses the default policy above
app.MapControllers();

app.Run();
