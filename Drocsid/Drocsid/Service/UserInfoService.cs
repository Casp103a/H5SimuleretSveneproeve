using Drocsid.Messages.Events;
using LogicLibrary.Dtos;

namespace Drocsid.Service
{
    internal interface IUserInfoService
    {
        UserDto? CurrentUser { get; set; }
        void Signout();
    }

    internal class UserInfoService : IUserInfoService
    {
        private readonly IEventAggregator eventAggregator;

        public UserInfoService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<SignoutEvent>().Subscribe(Signout);
            this.eventAggregator.GetEvent<DisposeEvent>().Subscribe(Dispose);
        }

        public UserDto? CurrentUser { get; set; }

        public void Signout()
        {
            CurrentUser = null;
        }

        private void Dispose()
        {
            Signout();
            eventAggregator.GetEvent<SignoutEvent>().Unsubscribe(Signout);
            eventAggregator.GetEvent<DisposeEvent>().Unsubscribe(Dispose);
        }
    }
}
