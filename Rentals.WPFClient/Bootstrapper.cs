using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using Rentals.WPFClient.Dashboard;
using Rentals.WPFClient.Modules;
using Rentals.WPFClient.Views;
using System.Windows;

namespace Rentals.WPFClient
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)ModuleCatalog;

            catalog.AddModule(typeof(ModulesModule));
            catalog.AddModule(typeof(DashboardModule));
        }
    }
}
