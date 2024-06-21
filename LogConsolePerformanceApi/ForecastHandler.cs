namespace LogConsolePerformanceApi;

internal sealed partial class ForecastHandler(ILogger<ForecastHandler> logger)
{
    private readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public IList<WeatherForecast> HandleRequest()
    {
        logger.LogInformation("Get forecast");
        
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
    static partial void LogGetForecast(ILogger<ForecastHandler> logger, DateOnly? date = null,
        int? temperatureC = null, string? summary = null);
}