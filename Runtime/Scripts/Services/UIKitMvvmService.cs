using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.MVVM.Pipeline;

namespace TinaX.UIKit.MVVM.Services
{
    public class UIKitMvvmService : IUIKitMvvmService
    {
        /// <summary>
        /// Binding query recursion depth
        /// 绑定查询递归深度
        /// </summary>
        public int BindingQueryRecursionDepth { get; set; } = 5;

        public XPipeline<IViewModelHandler> ViewModeHandlers { get; private set; } = new XPipeline<IViewModelHandler>();



    }
}
