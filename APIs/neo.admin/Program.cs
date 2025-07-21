using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Data.FaskesObj.Factories;
using neo.admin.Facades;
using neo.admin.Migrations.Factories;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Common;
using Shared.Communication.DependencyInjection;
using Shared.EFCore;
using Shared.Entities.Queries;
using Shared.Entities.Queries.Enterprise;
using Shared.Logging;
using Shared.Mailing;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;


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

/*  Load DBContexts */
// Register the DbContext
b.Services.AddDbContext<EnterpriseDbContext>(o =>
    o.UseNpgsql(b.Configuration.GetConnectionString("EnterpriseDB")));

// Register each interface mapping to the same instance
b.Services.AddScoped<IEnterpriseDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IPreRegistDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IOtpTokenDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("sys_corporate", "sys_faskes", "sys_login", "sys_auth_session");
/* --------------------------------------------- */

/*  Load Factories  */
b.Services.AddScoped<FaskesDbContextFactory>();
b.Services.AddScoped<DbProvisionerFactory>();

/*  Load base libraries */
b.Services.AddSharedRestClient();         // registers RestClient
b.Services.AddMailing(b.Configuration);   // SMTP

/*  Load services   */
b.Services.AddScoped<ICaptchaValidatorService, CaptchaValidatorService>();
b.Services.AddScoped<MailService>();

/*  Load queries    */
b.Services.AddScoped<FaskesQueries>();
b.Services.AddScoped<CorporateQueries>();
b.Services.AddScoped<LoginQueries>();
b.Services.AddScoped<FaskesQueries>();
b.Services.AddScoped<CorporateQueries>();
b.Services.AddScoped<OtpTokenQueries>();
b.Services.AddScoped<PreRegistQueries>();

/*  Load facades    */
b.Services.AddScoped<IRegistrationFacade, RegistrationFacade>();
b.Services.AddScoped<IResetPasswordFacade, ResetPasswordFacade>();

/*  Set Cors Policy for UI    */
b.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(b.Configuration.GetValue<string>("App:RegisterWebUrl") ?? Constants.REGISTER_FRONT_END_URL)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = b.Build();

app.UseCors();   // uses the default policy above

/*  Endpoints   */

/* 1. GET faskes by nomor */
app.MapGet("/faskes/search/{noFaskes}",
    async (string noFaskes, FaskesQueries q, CancellationToken ct) => {
        var faskes = await q.GetAsync(noFaskes, ct);

        if(faskes == null)
            return Results.NoContent();

        var info = new FaskesInfoResponse(
            faskes.Id, faskes.Name, faskes.Email, faskes.Phone,
            faskes.EmailBill, faskes.PhoneBill, faskes.EmailTech, faskes.PhoneTech,
            faskes.Address, faskes.IsActive, faskes.CorporateId, faskes.Name
        );
        
        return Results.Ok(info);
});

/* 2. GET corporations search */
app.MapGet("/corporates",
    async (string q, CorporateQueries cqs, CancellationToken ct) =>
        Results.Ok(await cqs.SearchAsync(q, ct)));

/* 3. POST register faskes */
app.MapPost("/faskes/register",
    async (RegisterFaskesRequest req,
           IRegistrationFacade facade,
           CancellationToken ct) =>
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(req);

        if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
        {
            return Results.BadRequest(validationResults);
        }

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

app.MapPost("reset-password",
    async (ResetPasswordReq req,
           IResetPasswordFacade facade,
           CancellationToken ct) =>
    {
        var otpIsValid = await facade.IsOtpValidAsync(req.Otp, ct);

        if (!otpIsValid.Success)
            return Results.BadRequest(otpIsValid); // Prompt user to ask for new OTP

        await facade.MarkIsUsedAsync(req.Otp, ct);  // Expire the current OTP

        // Validate password match
        if (req.Password != req.RePassword)
            return Results.BadRequest(new CommonAPIBodyResponse()
            {
                Success = false,
                Message = "Passwords do not match." 
            });

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var result = await facade.UpdatePasswordAsync(req.Otp, hashedPassword, ct);

        if (result.Success)
        {
            return Results.Ok(result);
        }
        else
        {
            return Results.UnprocessableEntity(result);
        }
    });

app.Run();
