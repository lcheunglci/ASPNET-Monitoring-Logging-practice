using eCommerce.Docker.Api;
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
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Assembly", name)
    .Enrich.FromLogContext()
    .WriteTo.Seq(serverUrl: "http://host.docker.internal:5341")
    .WriteTo.Console();
    // available sinks: https://github.com/serilog/serilog/wiki/Provided-Sinks
    // Seq: https://datalust.co/seq
    // Seq with Docker: https://docs.datalust.co/docs/getting-started-with-docker
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var connectionString = "hello"; // ConnectionString:Db
var connectionString = builder.Configuration.GetConnectionString("Db");
//var simpleProperty = "hey"; // SimpleProperty
var simpleProperty = builder.Configuration.GetValue<string>("SimpleProperty");
//var nestedProp = "here we go";  // Inventory->NestedProperty
var nestedProp = builder.Configuration.GetValue<string>("Inventory:NestedProperty");

//Log.ForContext("ConnectionString", connectionString)
//    .ForContext("SimpleProperty", simpleProperty)
//    .ForContext("Inventory:NestedProperty", nestedProp)
//    .ForContext("Loaded configuration!", connectionString);

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddScoped<IQuickOrderLogic, QuickOrderLogic>();

var app = builder.Build();

app.UseMiddleware<CustomExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var dbgView = (builder.Configuration as IConfigurationRoot).GetDebugView();
    Log.ForContext("ConfigurationDebug", dbgView)
        .Information("Configuration dump");

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
