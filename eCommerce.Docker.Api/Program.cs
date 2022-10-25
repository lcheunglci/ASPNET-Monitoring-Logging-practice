using eCommerce.Docker.Api.Domain;
using eCommerce.Docker.Api.Interfaces;
using eCommerce.Docker.Api.Middleware;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var name = typeof(Program).Assembly.GetName().Name;

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Assembly", name)
    .Enrich.FromLogContext();
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddScoped<IQuickOrderLogic, QuickOrderLogic>();

var app = builder.Build();

app.UseMiddleware<CustomExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
