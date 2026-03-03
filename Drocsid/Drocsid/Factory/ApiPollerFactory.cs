using LogicLibrary.Services;
using Microsoft.Extensions.Hosting;

namespace Drocsid.Factory
{
    internal interface IApiPollerFactory
    {
        BackgroundService Create(PollerConfig config);
    }

    internal class ApiPollerFactory : IApiPollerFactory
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IHttpClientService httpClientService;

        public ApiPollerFactory(IEventAggregator eventAggregator, IHttpClientService httpClientService)
        {
            this.eventAggregator = eventAggregator;
            this.httpClientService = httpClientService;
        }

        public BackgroundService Create(PollerConfig config)
        {
            return new ChatApiPoller(config, eventAggregator, httpClientService);
        }
    }
}
