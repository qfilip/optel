using System.Diagnostics;
using System.Diagnostics.Metrics;
using Telemetric.Shared.Models;

namespace Telemetric.Shared.Az;

public static class AzDiagnosticsConfig
{
    public const string ServiceName = "az";
    public static Meter Meter = new(ServiceName);
    private static Histogram<int> SalesValue = Meter.CreateHistogram<int>("request.value");
    private static Counter<long> SalesRequestCounter = Meter.CreateCounter<long>("request.requests");
    private static ActivitySource Source = new(ServiceName);

    public static class Metrics
    {
        public static void AddSalesRequestMetric(int productId, int price)
        {
            var labels = new KeyValuePair<string, object?>(Diagnostics.Product.Tags.Id, productId);

            SalesValue.Record(price, labels);
            SalesRequestCounter.Add(1, tag: labels);
        }
    }

    public static void StartActivity(this Activity? activity, ProductRequest request)
    {
        activity?.SetBaggage(Diagnostics.Product.Tags.Id, request.Id.ToString());

        activity?.Start()
            .SetTag(Diagnostics.Product.Tags.Id, request.Id)
            .SetTag(Diagnostics.Product.Tags.Price, request.Price);
    }

    public static void StartActivity(ProductRequest request)
    {
        using var activity = Source.StartActivity(Diagnostics.Product.Activity.Order);
        
        activity?.SetTag(Diagnostics.Product.Tags.Id, request.Id);
        activity?.SetTag(Diagnostics.Product.Tags.Price, request.Price);
        activity?.SetBaggage(Diagnostics.Product.Tags.Id, request.Id.ToString());
    }
}
