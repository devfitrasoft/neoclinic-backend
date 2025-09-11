using Microsoft.EntityFrameworkCore;
using neo.scheduler.Data.Enterprise;
using neo.scheduler.ScheduledProcessor;
using Shared.Entities.Queries;

var b = WebApplication.CreateBuilder(args);

b.WebHost
    .UseKestrel()
    .UseIISIntegration();

b.Services.AddDbContext<EnterpriseDbContext>(o =>
    o.UseNpgsql(b.Configuration.GetConnectionString("EnterpriseDB")));

b.Services.AddScoped<IOtpTokenDbContext>(sp => sp.GetRequiredService<EnterpriseDbContext>());

b.Services.AddSingleton<IHostedService, PurgingTask>();

var app = b.Build();

app.Run();
