using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

app.MapGet("/", () => Results.Ok("Working"));

app.MapPost("/order", async ([FromBody] ProductRequest request, [FromServices] BukiClient bukiClient) =>
{
    AzDiagnosticsConfig.StartActivity(Activity.Current, request);
    await bukiClient.OrderAsync(request);
    AzDiagnosticsConfig.Metrics.AddSalesRequestMetric(request.Id, request.Price);

    Results.Ok();
});

app.Run();
