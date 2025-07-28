using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neo.preregist.Common;
using neo.preregist.Data.Enterprise;
using neo.preregist.Facades;
using neo.preregist.Models;
using neo.preregist.Services;
using Shared.Common;
using Shared.EFCore;
using Shared.Entities.Queries;
using Shared.Entities.Queries.Enterprise;
using Shared.Logging;
using Shared.Mailing;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

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

/* Register queries that depend on those interfaces */
b.Services.AddScoped<PreRegistQueries>();
b.Services.AddScoped<OtpTokenQueries>();

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("pre_regist", "sys_otp");
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
            return Results.Json(new TokenVerifyResponse()
            {
                Success = false,
                Message = StringParser.ValidationErrorMessageBuilder(validationResults)
            }, statusCode: StatusCodes.Status400BadRequest);

        var res = await facade.SaveAndNotify(req, ct);

        if (res.Data.FirstOrDefault() != null)
        {
            return res.Data.FirstOrDefault()?.status switch
            {
                PreRegistSaveResponse.Created => Results.Created("/pre-register", res),
                PreRegistSaveResponse.Updated => Results.Ok(res),
                PreRegistSaveResponse.Registered => Results.Json(res, statusCode: StatusCodes.Status202Accepted),
                PreRegistSaveResponse.Error => Results.Json(res, statusCode: StatusCodes.Status500InternalServerError),
                _ => Results.Json(res, statusCode: StatusCodes.Status500InternalServerError)
            };
        }
        else
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    });

app.MapGet("pre-register/verify",
    async ([FromQuery] string token, IPreRegistFacade facade, CancellationToken ct) =>
    {
        var row = await facade.GetRowByTokenAsync(token, ct);

        if (row == null)
            return Results.Json(new TokenVerifyResponse()
            {
                Success = false,
                Message = "Token unauthorized"
            }, statusCode: StatusCodes.Status401Unauthorized); // Token expired

        //  3. Check if token is expired
        if (!row.otpExpiresAt.HasValue || row.otpExpiresAt.Value < DateTime.UtcNow)
            return Results.Json(new TokenVerifyResponse()
            {
                Success = false,
                Message = "Token unauthorized"
            }, statusCode: StatusCodes.Status401Unauthorized); // Token expired

        //  4. Check if is_registered_web = true
        if(row.isRegistered)
            return Results.Json(new TokenVerifyResponse()
            {
                Success = false,
                Message = "Data already registered into the system"
            }, statusCode: StatusCodes.Status409Conflict); // Account already registered, henche conflict

        //  5. Return essential data
        return Results.Ok(new TokenVerifyResponse()
        {
            Success = true,
            Data = new List<PreRegistData>() { row },
        });
    });

app.Run();
