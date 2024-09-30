using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Telemetric.Shared.Buki;

namespace Telemetric.Buki;

public static class Telemetry
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(o => o.AddOtlpExporter());

        var zipkinHostName = Environment.GetEnvironmentVariable("ZIPKIN_HOSTNAME");
        if (zipkinHostName == null)
            throw new InvalidOperationException("Zipkin hostname not found");

        builder.Services.AddLogging(lb =>
        {
            lb.AddOpenTelemetry(o =>
            {
                o.SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddService(BukiDiagnosticsConfig.ServiceName))
                    .AddOtlpExporter();
            });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb =>
                rb
                    .AddService(BukiDiagnosticsConfig.ServiceName)
                    .AddAttributes(new List<KeyValuePair<string, object>>
                    {
                        new("env", "dev")
                    }))
            .WithTracing(tpb =>
                tpb
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter()
                    .AddZipkinExporter(b => b.Endpoint = new Uri($"http://{zipkinHostName}:9411/api/v2/spans")))
            .WithMetrics(mpb =>
                mpb
                    .AddMeter(BukiDiagnosticsConfig.Meter.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter());
    }
}
