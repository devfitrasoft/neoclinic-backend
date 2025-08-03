using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Data.FaskesObj.Factories;
using neo.admin.Facades;
using neo.admin.Migrations.Factories;
using neo.admin.Models;
using neo.admin.Services;
using neo.admin.Services.Factories;
using neo.admin.Services.Token;
using neo.admin.StartupActions;
using Shared.Common;
using Shared.Communication.DependencyInjection;
using Shared.EFCore;
using Shared.Entities.Queries;
using Shared.Entities.Queries.Enterprise;
using Shared.Logging;
using Shared.Mailing;
using Shared.Models;
using System.ComponentModel.DataAnnotations;


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
b.Services.AddScoped<IEnterpriseDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IPreRegistDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IOtpTokenDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IBillingDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());
b.Services.AddScoped<IPICDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());

/* ------------ automatic migration ------------ */
b.Services.AddEfAutoMigration<EnterpriseDbContext>("sys_billing_setting", "sys_corporate", 
    "sys_faskes", "sys_billing", "sys_login", "sys_auth_session", "sys_pic");
/* --------------------------------------------- */

/*  Load Factories  */
b.Services.AddScoped<FaskesDbContextFactory>();
b.Services.AddScoped<DbProvisionerFactory>();

/*  Load base libraries */
b.Services.AddSharedRestClient();         // registers RestClient
b.Services.AddMailing(b.Configuration);   // SMTP

/*  Load token-related services */
b.Services.AddScoped<IJwtProvider, JwtProvider>();
b.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
b.Services.AddScoped<ITokenService, TokenService>();

/*  Load services   */
b.Services.AddScoped<ICaptchaValidatorService, CaptchaValidatorService>();
b.Services.AddScoped<MailService>();

/*  Load queries    */
b.Services.AddScoped<PICQueries>();
b.Services.AddScoped<LoginQueries>();
b.Services.AddScoped<FaskesQueries>();
b.Services.AddScoped<CorporateQueries>();
b.Services.AddScoped<OtpTokenQueries>();
b.Services.AddScoped<PreRegistQueries>();
b.Services.AddScoped<BillingSettingQueries>();
b.Services.AddScoped<BillingQueries>();

/*  Load facades    */
b.Services.AddScoped<IRegistrationFacade, RegistrationFacade>();
b.Services.AddScoped<IResetPasswordFacade, ResetPasswordFacade>();

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

/*  Endpoints   */

/* 1. GET faskes by nomor */
app.MapGet("/faskes/search/{noFaskes}",
    async (string noFaskes, FaskesQueries q, CancellationToken ct) => {
        var faskes = await q.GetNotDeletedAsync(noFaskes, ct);

        if (faskes == null)
            return Results.Json(new FaskesInfoResponseModel()
            {
                Success = false,
                Message = "Couldn't find data faskes for the requested nomor faskes"
            }, statusCode: StatusCodes.Status204NoContent);

        var data = new FaskesInfoResponseDataModel(
            faskes.Id, faskes.Name, faskes.Email, faskes.Phone,
            faskes.Address, faskes.IsActive, faskes.CorporateId, faskes.Name
        );

        return Results.Ok(new FaskesInfoResponseModel()
        {
            Success = true,
            Data = new List<FaskesInfoResponseDataModel>() { data }
        });
});

/* 2. GET corporations search */
app.MapGet("/corporates",
    async (string q, CorporateQueries cqs, CancellationToken ct) =>
    {
        var corporateList = await cqs.SearchNotDeletedAsync(q, ct);

        if (corporateList == null)
            return Results.Json(new CorporateInfoResponseModel() 
            { 
                Success = false, 
                Message = "No corporate data available"
            }, statusCode: StatusCodes.Status204NoContent);


        return Results.Ok(new CorporateInfoResponseModel()
        {
            Success = true,
            Data = corporateList
        });
    });

/* 3. POST register faskes */
app.MapPost("/faskes/register",
    async ([FromQuery] string otp,
           [FromBody] RegisterFaskesRequest req,
           IRegistrationFacade facade,
           CancellationToken ct) =>
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(req);

        if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
            return Results.Json(new RegisterFaskesResponseModel()
            {
                Success = false,
                Message = StringParser.ValidationErrorMessageBuilder(validationResults)
            }, statusCode: StatusCodes.Status400BadRequest);

        var res = await facade.RegisterAsync(otp, req, ct);
        return Results.Json(res, statusCode: StatusCodes.Status201Created);
    });

/* GET Back‑office activation */
app.MapGet("/faskes/activate/{username}",
    async (string username,
           IRegistrationFacade facade,
           CancellationToken ct) =>
    {
        bool isActivated = await facade.ActivateFaskesAsync(username, ct);

        if (isActivated)
        {
            return Results.Ok(new CommonAPIBodyResponseModel(){
                Success = true
            });
        }
        else
        {
            return Results.Json(new CommonAPIBodyResponseModel()
            {
                Success = false,
                Message = "One or some of the object couldn't be saved/updated to the database"
            }, statusCode:StatusCodes.Status500InternalServerError);
        }
    });

app.MapPost("reset-password",
    async (ResetPasswordReq req,
           IResetPasswordFacade facade,
           CancellationToken ct) =>
    {
        var otpIsValid = await facade.IsOtpValidAsync(req.Otp, ct);

        if (!otpIsValid.Success)
            return Results.BadRequest(otpIsValid); // Prompt user to ask for new OTP

        int resOtpUsage = await facade.MarkIsUsedAsync(req.Otp, ct);  // Expire the current OTP

        if(resOtpUsage == 0)
            return Results.BadRequest(new CommonAPIBodyResponseModel()
            {
                Success = false,
                Message = "Invalid OTP"
            });

        if (resOtpUsage == 2)
            return Results.BadRequest(new CommonAPIBodyResponseModel()
            {
                Success = false,
                Message = "OTP has been used"
            });

        // Validate password match
        if (req.Password != req.RePassword)
            return Results.BadRequest(new CommonAPIBodyResponseModel()
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

app.MapControllers();

app.Run();
