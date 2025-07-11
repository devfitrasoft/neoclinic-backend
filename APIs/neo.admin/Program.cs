using Microsoft.EntityFrameworkCore;
using neo.admin;
using neo.admin.Data.Enterprise;
using neo.admin.Data.FaskesObj.Factories;
using neo.admin.Data.Services;
using neo.admin.Facades;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Communication.DependencyInjection;
using Shared.EFCore;
using Shared.Logging;
using Shared.Mailing;


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

b.Services.AddScoped<FaskesDbContextFactory>();

b.Services.AddSharedRestClient();         // registers RestClient
b.Services.AddMailing(b.Configuration);   // SMTP
b.Services.AddScoped<ICaptchaValidator, CaptchaValidatorService>();
b.Services.AddScoped<FaskesQueryService>();
b.Services.AddScoped<CorporateQueryService>();
b.Services.AddScoped<FaskesDbProvisionerService>();
b.Services.AddScoped<MailService>();

b.Services.AddScoped<IRegistrationFacade, RegistrationFacade>();

b.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(b.Configuration.GetValue<string>("App:FrontendUrl") ?? Constants.FRONT_END_URL)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = b.Build();

app.UseCors();   // uses the default policy above

/* 1. GET faskes by nomor */
app.MapGet("/faskes/search/{noFaskes}",
    async (string noFaskes, FaskesQueryService q, CancellationToken ct) =>
        await q.GetAsync(noFaskes, ct) is { } info
            ? Results.Ok(info)
            : Results.Ok());

/* 2. GET corporations search */
app.MapGet("/corporates",
    async (string q, CorporateQueryService cqs, CancellationToken ct) =>
        Results.Ok(await cqs.SearchAsync(q, ct)));

/* 3. POST register faskes */
app.MapPost("/faskes/register",
    async (RegisterFaskesRequest req,
           IRegistrationFacade facade,
           CancellationToken ct) =>
    {
        var res = await facade.RegisterAsync(req, ct);
        return Results.Created($"/faskes/register", res);
    });

/* GET Back‑office activation */
app.MapGet("/faskes/activate/{username}",
    async (string username,
           IRegistrationFacade facade,
           CancellationToken ct) =>
    {
        await facade.ActivateFaskesAsync(username, ct);
        return Results.Ok();
    });

app.Run();
