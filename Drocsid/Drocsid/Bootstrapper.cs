using Drocsid.Views;
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
        }
    }
}
