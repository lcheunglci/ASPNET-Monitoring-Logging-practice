using eCommerce.Api;
using eCommerce.Data;
using eCommerce.Domain;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
//using NLog;
//using NLog.Web;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
//builder.Logging.AddSimpleConsole();
// builder.Logging.AddJsonConsole();
// builder.Logging.AddDebug();
// builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Seq("http://localhost:5341");
});

builder.Services.AddOpenTelemetryTracing(b =>
{
    b.SetResourceBuilder(
        ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddAspNetCoreInstrumentation()
    .AddEntityFrameworkCoreInstrumentation()
    .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
});

// note: http://localhost:4317 is for the jaeger container
// using the following command 
// docker run --name jaeger -p 13133:13133 -p 16686:16686 -p 4317:55680 -d --restart=unless-stopped jaegertracing/opentelemetry-all-in-one



//NLog.LogManager.Setup().LoadConfigurationFromFile();
//builder.Host.UseNLog();

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (context, ex) => false;
    options.OnBeforeWriteDetails = (context, details) =>
    {
        if (details.Status == 500)
        {
            details.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    };
    options.Rethrow<SqliteException>();
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        options.Audience = "api";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "email"
        };
    });

//builder.Logging.AddFilter("eCommerce", LogLevel.Debug);

//var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
//var tracePath = Path.Join(path, $"Log_eCommerce_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
//Trace.Listeners.Add(new TextWriterTraceListener(System.IO.File.CreateText(tracePath)));
//Trace.AutoFlush = true;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<IECommerceRepository, ECommerceRepository>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<LocalContext>();


var app = builder.Build();
app.UseProblemDetails();
app.UseMiddleware<CriticalExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LocalContext>();
    context.MigrateAndCreateData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("interactive.public.short");
        options.OAuthAppName("eCommerce API");
        options.OAuthUsePkce();
    });
}
app.MapFallback(() => Results.Redirect("/swagger"));
// temp disabled for using docker seq on localhost for http
// app.UseHttpsRedirection();
// note: Seq.Input.HealthCheck is the nuget package used on Seq for this to work
// also change localhost:5341 to host.docker.internal:5341 in the target URLs under Apps instance in Seq
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.MapHealthChecks("health").AllowAnonymous();
app.Run();
