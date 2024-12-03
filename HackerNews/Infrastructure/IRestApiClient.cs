namespace HackerNews.Infrastructure
{

    /// <summary>
    /// IRestApiClient makes abstraction over HttpClient and allows easily swap with other implementation. 
    /// </summary>
    public interface IRestApiClient
    {
        Task<T?> GetAsync<T>(string url);
    }
}
