using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();
    builder.Services.AddSingleton<ForecastHandler>();

    if (builder.Configuration.GetValue<bool>("Logger:ApplicationInsights:Enabled"))
    {
        builder.Services.AddApplicationInsightsTelemetry();
    }

    builder.Host.UseSerilog((hostBuilderContext, services, loggerConfiguration) =>
    {
        loggerConfiguration.MinimumLevel.Information();

        if (hostBuilderContext.Configuration.GetValue<bool>("Logger:Console:Enabled"))
        {
            loggerConfiguration.WriteTo.Console(outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        if (hostBuilderContext.Configuration.GetValue<bool>("Logger:ApplicationInsights:Enabled"))
        {
            var telemetryConfiguration = services.GetRequiredService<TelemetryConfiguration>();
            telemetryConfiguration.ConnectionString =
                builder.Configuration["Logger:ApplicationInsights:ConnectionString"];
            loggerConfiguration
                .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces);
        }

        loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
    });
    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapGet("/", (ForecastHandler handler) => handler.HandleRequest());

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

internal sealed partial class ForecastHandler(ILogger<ForecastHandler> logger)
{
    private readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public IList<WeatherForecast> HandleRequest()
    {
        LogGetForecast(logger);
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    _summaries[Random.Shared.Next(_summaries.Length)]
                ))
            .ToArray();
        foreach (var f in forecast)
        {
            LogGetForecast(logger, f.Date, f.TemperatureC, f.Summary);
        }

        return forecast;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Get forecast {Date} {TemperatureC} {Summary}")]
    public static partial void LogGetForecast(ILogger<ForecastHandler> logger, DateOnly? date = null,
        int? temperatureC = null, string? summary = null);
}

internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}