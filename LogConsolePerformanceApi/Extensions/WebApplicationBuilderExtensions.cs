using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

namespace LogConsolePerformanceApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureLoggingAndTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
        
        if (!string.IsNullOrWhiteSpace(builder.Configuration["ApplicationInsights:ConnectionString"]))
        {
            builder.Services.AddApplicationInsightsTelemetry();
        }

        builder.Host.UseSerilog((hostBuilderContext, services, loggerConfiguration) =>
        {
            if (!string.IsNullOrWhiteSpace(hostBuilderContext.Configuration["ApplicationInsights:ConnectionString"]))
            {
                var telemetryConfiguration = services.GetRequiredService<TelemetryConfiguration>();
                telemetryConfiguration.ConnectionString =
                    hostBuilderContext.Configuration["ApplicationInsights:ConnectionString"];
                loggerConfiguration
                    .WriteTo.ApplicationInsights(
                        telemetryConfiguration: services.GetRequiredService<TelemetryConfiguration>(),
                        telemetryConverter: TelemetryConverter.Traces);
            }

            loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
        });
    }
}