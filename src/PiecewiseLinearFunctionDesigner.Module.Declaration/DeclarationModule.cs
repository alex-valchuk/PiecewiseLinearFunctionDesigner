using PiecewiseLinearFunctionDesigner.Module.Declaration.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration
{
    public class DeclarationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("DeclarationRegion", typeof(FunctionListView));
            regionManager.RegisterViewWithRegion("DeclarationSelectedFunctionRegion", typeof(FunctionView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}