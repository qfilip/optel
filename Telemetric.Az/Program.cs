using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using Telemetric.Az;
using Telemetric.Shared.Az;
using Telemetric.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Telemetry.Configure(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("/order", ([FromBody] ProductRequest request) =>
{
    using var activity = AzDiagnosticsConfig.Source.StartActivity("product_order");
    activity!.EnrichProductOrderRequest(request);
    AzDiagnosticsConfig.Metrics.AddSalesMetric(request.Id, request.Price);

});

app.Run();
