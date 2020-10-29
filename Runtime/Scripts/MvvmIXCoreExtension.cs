using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.MVVM.Pipeline;
using TinaX.UIKit.MVVM.Services;

namespace TinaX.Services
{
    public static class MvvmIXCoreExtension
    {
        public static IXCore UseUIKitMVVM(this IXCore core)
        {
            core.Services.Singleton<IUIKitMvvmService, UIKitMvvmService>();

            //ViewModel Handler
            core.Services.Get<IUIKitMvvmService>().ViewModeHandlers.AddLast(new XComponentViewModeHandler());

            return core;
        }
    }
}
