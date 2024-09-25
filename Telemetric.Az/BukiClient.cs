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
        var message = new HttpRequestMessage();
        
        message.Method = HttpMethod.Post;
        message.RequestUri = new Uri("http:localhost:50001/order");
        message.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }
}
