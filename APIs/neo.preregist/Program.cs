using Microsoft.EntityFrameworkCore;
using neo.preregist.Data.Enterprise;
using neo.preregist.Facades;
using neo.preregist.Models;
using neo.preregist.Services;
using Shared.Common;
using Shared.Mailing;
using Shared.EFCore;
using Shared.Logging;
using System.ComponentModel.DataAnnotations;
using neo.admin.Common;

var b = WebApplication.CreateBuilder(args);

/* using Shared.Logging library */
LoggingSetup.ConfigureBootstrapLogger(b.Configuration);
b.Host.UseSerilogLogging(b.Configuration);

// add worker only when file sink active
if (b.Configuration.GetValue("Logging:EnableFileSink", true))
    b.Services.AddHostedService<LogMaintenanceWorker>();  // register background compressor

b.Services.AddDbContext<EnterpriseDbContext>(o =>
    o.UseNpgsql(b.Configuration.GetConnectionString("EnterpriseDB")));

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("pre_regist");
/* --------------------------------------------- */

/*  Load base libraries */
b.Services.AddMailing(b.Configuration);   // SMTP

/*  Load services   */
b.Services.AddScoped<MailService>();

/*  Load facades    */
b.Services.AddScoped<IPreRegistFacade, PreRegistFacades>();

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

var app = b.Build();

app.UseCors();   // uses the default policy above

/*  Endpoints   */

/*  1. Pre-register form in Landing page */
app.MapPost("/pre-register",
    async (PreRegistRequest req,
           IPreRegistFacade facade,
           CancellationToken ct) =>
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(req);

        if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
        {
            return Results.BadRequest(validationResults);
        }

        var res = await facade.SaveAndNotify(req, ct);

        if (res != null)
        {
            return res.Status switch
            {
                PreRegistSaveResponse.Created => Results.Created("/pre-register", res),
                PreRegistSaveResponse.Updated => Results.Ok(res),
                PreRegistSaveResponse.Error => Results.Json(res, statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Json(res, statusCode: StatusCodes.Status500InternalServerError)
            };
        }
        else
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    });

app.Run();
