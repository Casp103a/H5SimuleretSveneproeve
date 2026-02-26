using Drocsid.Views;
using System.Windows;

namespace Drocsid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Bootstrapper? bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            var region = bootstrapper.Container.Resolve<IRegionManager>();
            region.RequestNavigate("main", nameof(StartView));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (bootstrapper != null)
                bootstrapper.Dispose();

            base.OnExit(e);
        }
    }
}
