namespace LogicLibrary.Services
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent httpContent);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent httpContent);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<HttpResponseMessage> GetAsync(string url);
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient httpClient;

        public HttpClientService()
        {
            httpClient = new HttpClient();
        }

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return httpClient.DeleteAsync(url);
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return httpClient.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent httpContent)
        {
            return httpClient.PostAsync(url, httpContent);
        }

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent httpContent)
        {
            return httpClient.PutAsync(url, httpContent);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return httpClient.SendAsync(request);
        }
    }
}
