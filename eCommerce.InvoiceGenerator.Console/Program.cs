﻿// See https://aka.ms/new-console-template for more information
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        ConfigureLogging();

        try
        {
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