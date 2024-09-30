using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Telemetric.Shared.Buki;
using Telemetric.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("order", ([FromBody] ProductRequest request) =>
{
    BukiDiagnosticsConfig.StartActivity(Activity.Current, request);

    InMemoryDb.Requests.Add(request);

    BukiDiagnosticsConfig.Metrics.AddSalesMetric(request.Id, request.Price);
    
    return Results.Ok();
});

app.Run();

static class InMemoryDb
{
    public static List<ProductRequest> Requests { get; set; } = new();
}