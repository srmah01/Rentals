using Prism.Modularity;
using Prism.Regions;
using System;

namespace Rentals.WPFClient.Modules
{
    public class ModulesModule : IModule
    {
        
        IRegionManager _regionManager;

        public ModulesModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            InitialiseContainer();
            RegisterViewsAndRegions();
        }

        private void InitialiseContainer()
        {
            
        }

        private void RegisterViewsAndRegions()
        {
            _regionManager.RegisterViewWithRegion("TopMenu", typeof(Views.MainMenu));
            _regionManager.RegisterViewWithRegion("Footer", typeof(Views.Footer));
        }

    }
}