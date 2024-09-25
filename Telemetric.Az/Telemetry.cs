using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Telemetric.Shared.Az;

namespace Telemetric.Az;

public static class Telemetry
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(o => o.AddOtlpExporter());

        builder.Services.AddLogging(lb =>
        {
            lb.AddOpenTelemetry(o =>
            {
                o.SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddService(AzDiagnosticsConfig.ServiceName))
                    .AddOtlpExporter();
            });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => 
                rb
                    .AddService(AzDiagnosticsConfig.ServiceName)
                    .AddAttributes(new List<KeyValuePair<string, object>>
                    {
                        new("env", "dev")
                    }))
            .WithTracing(tpb =>
                tpb
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter())
            .WithMetrics(mpb =>
                mpb
                    .AddMeter(AzDiagnosticsConfig.Meter.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter());
    }
}
