using eCommerce.Api;
using eCommerce.Data;
using eCommerce.Domain;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
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

//builder.Logging.AddFilter("eCommerce", LogLevel.Debug);

//var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
//var tracePath = Path.Join(path, $"Log_eCommerce_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
//Trace.Listeners.Add(new TextWriterTraceListener(System.IO.File.CreateText(tracePath)));
//Trace.AutoFlush = true;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<IECommerceRepository, ECommerceRepository>();


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
    app.UseSwaggerUI();
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
