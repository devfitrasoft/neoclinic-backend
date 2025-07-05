using Microsoft.EntityFrameworkCore;
using neo.admin.Data;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Mailing;
using Shared.Communication.DependencyInjection;
using Shared.Logging;
using Shared.EFCore;


var b = WebApplication.CreateBuilder(args);

/* Registration configs */
b.Services.Configure<RegistrationSettings>(
    b.Configuration.GetSection("Registration"));

/* using Shared.Logging library */
LoggingSetup.ConfigureBootstrapLogger(b.Configuration);
b.Host.UseSerilogLogging(b.Configuration);

// add worker only when file sink active
if (b.Configuration.GetValue("Logging:EnableFileSink", true))
    b.Services.AddHostedService<LogMaintenanceWorker>();  // register background compressor

b.Services.AddDbContext<EnterpriseDbContext>(o =>
    o.UseNpgsql(b.Configuration.GetConnectionString("EnterpriseDB")));

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("sys_corporate", "sys_faskes", "sys_login");
/* --------------------------------------------- */

b.Services.AddSharedRestClient();         // registers RestClient
b.Services.AddMailing(b.Configuration);   // SMTP
b.Services.AddScoped<IDBManagementClient, DBManagementClient>();
b.Services.AddScoped<ICaptchaValidator, GoogleCaptchaValidator>();
b.Services.AddScoped<RegistrationService>();
b.Services.AddScoped<FaskesQueryService>();
b.Services.AddScoped<CorporateQueryService>();
b.Services.AddScoped<RegistrationMailService>();

var app = b.Build();

/* 1. GET faskes by nomor */
app.MapGet("/faskes/{noFaskes}",
    async (string noFaskes, FaskesQueryService q, CancellationToken ct) =>
        await q.GetFaskesAsync(noFaskes, ct) is { } info
            ? Results.Ok(info)
            : Results.NotFound());

/* 2. GET corporations search */
app.MapGet("/corporates",
    async (string q, CorporateQueryService cqs, CancellationToken ct) =>
        Results.Ok(await cqs.SearchAsync(q, ct)));

/* 3. POST register faskes */
app.MapPost("/register/faskes",
    async (RegisterFaskesRequest req,
           RegistrationService svc,
           CancellationToken ct) =>
    {
        var res = await svc.RegisterAsync(req, ct);
        return Results.Created($"/register/faskes", res);
    });

app.Run();
