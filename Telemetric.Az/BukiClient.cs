using System.Text;
using System.Text.Json;
using Telemetric.Shared.Models;

namespace Telemetric.Az;

public class BukiClient
{
    private readonly HttpClient _httpClient;
    public BukiClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    public async Task OrderAsync(ProductRequest request)
    {
        var bukiAddress = Environment.GetEnvironmentVariable("BUKI_ADDRESS");
        if (bukiAddress == null)
            throw new InvalidOperationException("Buki address not set in environment");

        var message = new HttpRequestMessage();
        
        message.Method = HttpMethod.Post;
        message.RequestUri = new Uri($"{bukiAddress}/order");
        message.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }
}
