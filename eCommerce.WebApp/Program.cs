using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole();
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

//NLog.LogManager.Setup().LoadConfigurationFromFile();
//builder.Host.UseNLog();


JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://demo.duendesoftware.com";
    options.ClientId = "interactive.confidential";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "email"
    };
    options.SaveTokens = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
    .AddIdentityServer(new Uri("https://demo.duendesoftware.com"),
    failureStatus: HealthStatus.Degraded);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});
var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
builder.Services.AddW3CLogging(options =>
{
    options.LoggingFields = W3CLoggingFields.All;
    options.FileSizeLimit = 5 * 1024 * 1024;
    options.RetainedFileCountLimit = 2;
    options.FileName = "eCommerce-w3c-ui";
    options.LogDirectory = Path.Combine(path, "logs");
    options.FlushInterval = TimeSpan.FromSeconds(2);
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseHttpLogging();
// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
// temp disabled for using localhost without https for docker seq
//app.UseHsts();
//}
// temp disabled for the localhost without https for docker seq
//app.UseHttpsRedirection();
// note: Seq.Input.HealthCheck is the nuget package used on Seq for this to work
// also change localhost:5341 to host.docker.internal:5341 in the target URLs under Apps instance in Seq
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();
app.MapHealthChecks("health").AllowAnonymous();

app.Run();
