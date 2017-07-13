using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Rentals.WPFClient.Dashboard.Services;
using System;

namespace Rentals.WPFClient.Dashboard
{
    public class DashboardModule : IModule
    {
        IUnityContainer _container;
        IRegionManager _regionManager;

        public DashboardModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IDashboardService, DashboardService>();

            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(Views.Dashboard));
        }
    }
}