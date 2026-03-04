using Drocsid.Factory;

namespace Drocsid.Service
{
    internal interface IApiPollerCoordinatorService
    {
        void CreateAndStartPoller(PollerConfig config);
        void CancelPollers();
    }

    internal class ApiPollerCoordinatorService : IApiPollerCoordinatorService
    {
        private readonly IApiPollerFactory factory;

        public ApiPollerCoordinatorService(IApiPollerFactory factory)
        {
            this.factory = factory;
            chatApiPollers = [];
            cancellationToken = new CancellationTokenSource();
        }

        private CancellationTokenSource cancellationToken;
        private List<ChatApiPoller> chatApiPollers;

        public void CreateAndStartPoller(PollerConfig config)
        {
            var poller = factory.Create(config);
            if (poller is ChatApiPoller chat)
                Task.Run(() =>  chat.RunAsync(cancellationToken.Token));
            chatApiPollers.Add(poller);
        }

        public void CancelPollers()
        {
            cancellationToken.Cancel();
        }
    }
}
