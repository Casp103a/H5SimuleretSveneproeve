using Drocsid.Views;
using System.Windows;

namespace Drocsid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            var region = bootstrapper.Container.Resolve<IRegionManager>();
            region.RequestNavigate("main", nameof(StartView));
        }
    }
}
