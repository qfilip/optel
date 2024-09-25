using Microsoft.AspNetCore.Mvc; 
using Telemetric.Az;
using Telemetric.Shared.Az;
using Telemetric.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<BukiClient>();

Telemetry.Configure(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("/order", async ([FromBody] ProductRequest request, [FromServices] BukiClient bukiClient) =>
{
    using var activity = AzDiagnosticsConfig.Source.StartActivity("product.order");
    activity!.EnrichProductOrderRequest(request);
    AzDiagnosticsConfig.Metrics.AddSalesMetric(request.Id, request.Price);

    await bukiClient.OrderAsync(request);

    Results.Ok();
});

app.Run();
