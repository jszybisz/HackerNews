using System.Text.Json;

namespace HackerNews.Infrastructure;

public class RestApiClient(HttpClient httpClient) : IRestApiClient
{
   
    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await httpClient.GetStringAsync(url);
        return JsonSerializer.Deserialize<T>(response);
    }
}