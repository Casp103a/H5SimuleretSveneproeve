using Drocsid.Dialogs.Views;
using Drocsid.Factory;
using Drocsid.Messages.Events;
using Drocsid.Service;
using Drocsid.Views;
using LogicLibrary.Interface;
using LogicLibrary.Services;
using System.Windows;

namespace Drocsid
{
    public class Bootstrapper : PrismBootstrapperBase, IDisposable
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterForNavigation<StartView>(nameof(StartView));
            container.RegisterForNavigation<RegisterUserView>(nameof(RegisterUserView));
            container.RegisterForNavigation<ChatOverviewView>(nameof(ChatOverviewView));
            container.RegisterForNavigation<FriendRequestView>(nameof(FriendRequestView));
            container.RegisterForNavigation<ChatView>(nameof(ChatView));

            container.RegisterDialog<AddFriendDialog>(nameof(AddFriendDialog));

            container.RegisterSingleton<IApplicationSpecific, ApplicationSpecific>();
            container.RegisterScoped<IHttpClientService, HttpClientService>();
            container.RegisterSingleton<IUserInfoService, UserInfoService>();
            container.RegisterScoped<ILoginService, LoginService>();
            container.RegisterScoped<IPasswordService, PasswordService>();
            container.RegisterSingleton<IApiPollerFactory, ApiPollerFactory>();
            container.RegisterSingleton<IApiPollerCoordinatorService, ApiPollerCoordinatorService>();
        }

        public void Dispose()
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<DisposeEvent>().Publish();
        }
    }
}
