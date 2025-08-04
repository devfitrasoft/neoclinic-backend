using Microsoft.EntityFrameworkCore;
using neo.preregist.Data.Enterprise;
using neo.preregist.Services;
using Shared.Common;
using Shared.EFCore;
using Shared.Entities.Queries;
using Shared.Logging;
using Shared.Mailing;

var b = WebApplication.CreateBuilder(args);

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
b.Services.AddScoped<IPreRegistDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IOtpTokenDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("pre_regist", "sys_otp");
/* --------------------------------------------- */

/*  Load base libraries */
b.Services.AddMailing(b.Configuration);   // SMTP

/*  Load services   */
b.Services.AddScoped<MailService>();

/*  Set Cors Policy for UI    */
b.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                b.Configuration.GetValue<string>("App:RegisterWebUrl") ?? Constants.REGISTER_FRONT_END_URL,
                b.Configuration.GetValue<string>("App:PreRegWebUrl") ?? Constants.PREREGIST_FRONT_END_URL
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

b.Services.AddControllers();

var app = b.Build();

app.UseCors();   // uses the default policy above
app.MapControllers();

app.Run();
