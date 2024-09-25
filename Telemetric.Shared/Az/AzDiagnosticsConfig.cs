using System.Diagnostics;
using System.Diagnostics.Metrics;
using Telemetric.Shared.Models;

namespace Telemetric.Shared.Az;

public static class AzDiagnosticsConfig
{
    public const string ServiceName = "az";
    public static Meter Meter = new(ServiceName);
    public static Histogram<int> SalesValue = Meter.CreateHistogram<int>("request.value");
    public static Counter<long> SalesCounter = Meter.CreateCounter<long>("request.requests");
    public static ActivitySource Source = new ActivitySource(ServiceName);

    public static class Metrics
    {
        public static void AddSalesMetric(int productId, int price)
        {
            var labels = new KeyValuePair<string, object?>(Diagnostics.Product.Tags.Id, productId);

            SalesValue.Record(price, labels);
            SalesCounter.Add(1, tag: labels);
        }
    }

    public static void StartActivity(ProductRequest request)
    {
        using var activity = Source.StartActivity(Diagnostics.Product.Activity.Order);
        
        activity?.SetTag(Diagnostics.Product.Tags.Id, request.Id);
        activity?.SetTag(Diagnostics.Product.Tags.Price, request.Price);
        activity?.SetBaggage(Diagnostics.Product.Tags.Id, request.Id.ToString());
    }
}
