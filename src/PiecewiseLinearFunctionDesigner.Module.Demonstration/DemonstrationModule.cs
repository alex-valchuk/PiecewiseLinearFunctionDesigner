using PiecewiseLinearFunctionDesigner.Module.Demonstration.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration
{
    public class DemonstrationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("DemonstrationRegion", typeof(FunctionGraphView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}