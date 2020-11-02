using TinaX.Systems.Pipeline;
using TinaX.UIKit.MVVM.Components;
using TinaX.UIKit.MVVM.Services;

namespace TinaX.UIKit.MVVM.Pipeline
{
    public interface IViewModelHandler : IPipelineHandler<IViewModelHandler>
    {
        bool HandleViewModel(BinderHandlerBaseComponent handlerComponent, IUIKitMvvmService mvvmService);
    }
}
