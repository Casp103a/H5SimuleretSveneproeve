using LogicLibrary.Interface;

namespace LogicLibrary.Services
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent httpContent);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent httpContent);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<HttpResponseMessage> GetAsync(string url);
    }

    public class HttpClientService : IHttpClientService, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly IApplicationSpecific? applicationSpecific;


        public HttpClientService(IApplicationSpecific applicationSpecific)
        {
            httpClient = new AuthClient(applicationSpecific);
            httpClient.BaseAddress = new Uri(applicationSpecific.BaseUrl!);
            this.applicationSpecific = applicationSpecific;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return await httpClient.SendAsync(request, cancellationToken);
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

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
