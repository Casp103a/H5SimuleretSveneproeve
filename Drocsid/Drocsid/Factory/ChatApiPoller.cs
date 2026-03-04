using Drocsid.Messages.Events;
using LogicLibrary.Services;
using Microsoft.Extensions.Hosting;

namespace Drocsid.Factory
{
    internal class ChatApiPoller : BackgroundService
    {
        public ChatApiPoller(PollerConfig config, IEventAggregator eventAggregator, IHttpClientService httpClientService)
        {
            this.config = config;
            this.eventAggregator = eventAggregator;
            this.httpClientService = httpClientService;
        }

        private PollerConfig config;
        private IHttpClientService httpClientService;
        private IEventAggregator eventAggregator;

        public Task RunAsync(CancellationToken ct) => ExecuteAsync(ct);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(config.Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var response = await httpClientService.GetAsync(config.Url, stoppingToken);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync(stoppingToken);

                    // Or raise an event / call a handler
                    var args = new ChatGotNewMessageArgs()
                    {
                        ChatGuid = config.PollerId,
                        ChatId = config.ChatId
                    };

                    eventAggregator.GetEvent<ChatGotNewMessagesEvent>().Publish(args);

                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {

                }
            }
        }
    }
}
