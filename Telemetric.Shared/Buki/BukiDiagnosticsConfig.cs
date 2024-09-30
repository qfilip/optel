using System.Diagnostics;
using System.Diagnostics.Metrics;
using Telemetric.Shared.Models;

namespace Telemetric.Shared.Buki;

public static class BukiDiagnosticsConfig
{
    public const string ServiceName = "buki";
    public static Meter Meter = new(ServiceName);
    private static Histogram<int> SalesValue = Meter.CreateHistogram<int>("request.value");
    private static Counter<long> SalesCounter = Meter.CreateCounter<long>("request.requests");
    private static ActivitySource Source = new(ServiceName);

    public static class Metrics
    {
        public static void AddSalesMetric(int productId, int price)
        {
            var labels = new KeyValuePair<string, object?>(Diagnostics.Product.Tags.Id, productId);

            SalesValue.Record(price, labels);
            SalesCounter.Add(1, tag: labels);
        }
    }

    public static void StartActivity(this Activity? activity, ProductRequest request)
    {
        var id = activity?.GetBaggageItem(Diagnostics.Product.Tags.Id);
        Console.WriteLine($"Baggage {id}");
        
        activity?.Start()
            .SetTag(Diagnostics.Product.Tags.Id, id)
            .SetTag(Diagnostics.Product.Tags.Price, request.Price);
    }

    public static void StartActivity(ProductRequest request)
    {
        using var activity = Source.StartActivity(Diagnostics.Product.Activity.Order);

        var id = activity?.GetBaggageItem(Diagnostics.Product.Tags.Id);
        Console.WriteLine($"Baggage {id}");
        activity?.SetTag(Diagnostics.Product.Tags.Id, id);
        activity?.SetTag(Diagnostics.Product.Tags.Price, request.Price);
    }
}
