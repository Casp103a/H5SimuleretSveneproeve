using LogicLibrary.Services;

namespace Drocsid.Factory
{
    internal interface IApiPollerFactory
    {
        ChatApiPoller Create(PollerConfig config);
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

        public ChatApiPoller Create(PollerConfig config)
        {
            return new ChatApiPoller(config, eventAggregator, httpClientService);
        }
    }
}
