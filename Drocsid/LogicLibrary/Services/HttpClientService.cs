using LogicLibrary.Interface;

namespace LogicLibrary.Services
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent httpContent, string? baseurl = null);
        Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent httpContent, string? baseurl = null);
        Task<HttpResponseMessage> DeleteAsync(string endpoint, string? baseurl = null);
        Task<HttpResponseMessage> GetAsync(string endpoint, string? baseurl = null);
    }

    public class HttpClientService : IHttpClientService, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly IApplicationSpecific applicationSpecific;


        public HttpClientService(IApplicationSpecific applicationSpecific)
        {
            httpClient = new AuthClient(applicationSpecific);
            this.applicationSpecific = applicationSpecific;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return await httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(string endpoint, string? baseurl = null)
        {
            string url = baseurl ?? applicationSpecific.BaseUrl!;
            url += endpoint;
            return httpClient.DeleteAsync(url);
        }

        public Task<HttpResponseMessage> GetAsync(string endpoint, string? baseurl = null)
        {
            string url = baseurl ?? applicationSpecific.BaseUrl!;
            url += endpoint;
            return httpClient.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent httpContent, string? baseurl = null)
        {
            string url = baseurl ?? applicationSpecific.BaseUrl!;
            url += endpoint;
            return httpClient.PostAsync(url, httpContent);
        }

        public Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent httpContent, string? baseurl = null)
        {
            string url = baseurl ?? applicationSpecific.BaseUrl!;
            url += endpoint;
            return httpClient.PutAsync(url, httpContent);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
