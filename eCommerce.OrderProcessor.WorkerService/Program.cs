using eCommerce.OrderProcessor.WorkerService;
using Serilog;
using Serilog.Events;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        string? name = typeof(Program).Assembly.GetName().Name;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Assembly", name)
            .WriteTo.Seq("http://host.docker.internal:5341")
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting host");
            IHost host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            await host.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}