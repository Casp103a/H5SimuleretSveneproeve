using Drocsid.Messages.Events;
using LogicLibrary.Interface;

namespace Drocsid
{
    internal class ApplicationSpecific : IApplicationSpecific
    {
        private readonly IEventAggregator eventAggregator;

        public ApplicationSpecific(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<SignoutEvent>().Subscribe(Signout);
            this.eventAggregator.GetEvent<DisposeEvent>().Subscribe(Dispose);
        }

        private void Signout()
        {
            SaveToken(null);
        }

        private void SignUserOutMethod()
        {
            eventAggregator.GetEvent<SignoutEvent>().Publish();
        }

        private string? jwtToken;

        public Action SignUserOut => SignUserOutMethod;

        public Action<string?> SaveTokenInSecureStorage => SaveToken;

        public string? BaseUrl => "https://drocsid.exploit4all.com";

        public string GetToken()
        {
            return jwtToken ?? string.Empty;
        }

        private void SaveToken(string? token)
        {
            jwtToken = token;
        }

        private void Dispose()
        {
            Signout();
            eventAggregator.GetEvent<SignoutEvent>().Unsubscribe(Signout);
            eventAggregator.GetEvent<DisposeEvent>().Unsubscribe(Dispose);
        }
    }
}
