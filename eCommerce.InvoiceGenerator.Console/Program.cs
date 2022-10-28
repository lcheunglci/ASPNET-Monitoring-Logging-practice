// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Serilog;

internal class Program
{
    private static IConfiguration _config;
    private static void Main(string[] args)
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables() // IConfigurationBuilder
            .Build(); // IConfigurationRoot

        Console.WriteLine("Hello, World!");
        ConfigureLogging();

        try
        {
            //var connectionString = "hello"; // ConnectionString:Db
            var connectionString = _config.GetConnectionString("Db");
            //var simpleProperty = "hey"; // SimpleProperty
            var simpleProperty = _config.GetValue<string>("SimpleProperty");
            //var nestedProp = "here we go";  // Inventory->NestedProperty
            var nestedProp = _config.GetValue<string>("Inventory:NestedProperty");

            Log.ForContext("ConnectionString", connectionString)
                .ForContext("SimpleProperty", simpleProperty)
                .ForContext("Inventory:NestedProperty", nestedProp)
                .ForContext("Loaded configuration!", connectionString);

            Log.ForContext("Args", args)
               .Information("Starting program...");

            Console.WriteLine("Hello World!"); // do some invoice generation

            Log.Information("Finished execution!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Some kind of exception occurred.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    private static void ConfigureLogging()
    {
        var name = typeof(Program).Assembly.GetName().Name;

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Assembly", name)
            .WriteTo.Console()
            .WriteTo.Seq("http://host.docker.internal:5341")
            .CreateLogger();
    }
}