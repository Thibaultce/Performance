using LogConsolePerformanceApi.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting up");


try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.ConfigureLoggingAndTelemetry();
    
    builder.Services.AddSingleton<LogConsolePerformanceApi.ForecastHandler>();
    
    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapGet("/", (LogConsolePerformanceApi.ForecastHandler handler) => handler.HandleRequest());
    
    await app.RunAsync().ConfigureAwait(false);
    
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}