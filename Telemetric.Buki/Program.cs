using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
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

app.MapPost("order", ([FromBody] ProductRequest request, [FromServices] IWebHostEnvironment env) =>
{
    BukiDiagnosticsConfig.StartActivity(request);
    
    var data = JsonSerializer.Serialize(request);
    var file = Path.Combine(env.WebRootPath, "orders.txt");
    
    if(File.Exists(file))
        File.AppendAllText(file, data);
    else
    {
        var stream = File.Open(file, FileMode.OpenOrCreate);
        stream.Write(Encoding.UTF8.GetBytes(data));
    }

    BukiDiagnosticsConfig.Metrics.AddSalesMetric(request.Id, request.Price);
    
    return Results.Ok();
});

app.Run();