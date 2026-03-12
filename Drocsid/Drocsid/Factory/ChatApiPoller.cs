using Drocsid.Messages.Events;
using Drocsid.Messages.Query;
using LogicLibrary.Services;
using System.Windows;

namespace Drocsid.Factory
{
    internal class ChatApiPoller
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

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(config.Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var arg = new GetLatestMessageQueryArgs()
                    {
                        ChatId = config.ChatId,
                        ConversationName = config.PollerId
                    };

                    eventAggregator.GetEvent<GetLatestMessageQuery>().Publish(arg);

                    var response = await httpClientService.GetAsync(String.Format(config.Url, arg.Message.Id), stoppingToken);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync(stoppingToken);
                    var cloudConvert = bool.TryParse(content, out var result);
                    
                    if (!cloudConvert)
                        continue;

                    if (!result)
                        continue;

                    var args = new ChatGotNewMessageArgs()
                    {
                        ChatGuid = config.PollerId,
                        ChatId = config.ChatId
                    };

                    Application.Current.Dispatcher.Invoke(() => eventAggregator.GetEvent<ChatGotNewMessagesEvent>().Publish(args));
                    

                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {

                }
            }
        }
    }
}
