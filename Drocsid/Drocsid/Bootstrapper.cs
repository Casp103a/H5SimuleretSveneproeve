using Drocsid.Views;
using LogicLibrary.Interface;
using LogicLibrary.Services;
using System.Windows;

namespace Drocsid
{
    public class Bootstrapper : PrismBootstrapperBase
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

            container.RegisterScoped<IHttpClientService, HttpClientService>();
            container.RegisterScoped<IApplicationSpecific, ApplicationSpecific>();
            container.RegisterScoped<ILoginService, LoginService>();
            container.RegisterScoped<IPasswordService, PasswordService>();
        }
    }
}
