using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.MVVM.Pipeline;

namespace TinaX.UIKit.MVVM.Services
{
    public interface IUIKitMvvmService
    {
        /// <summary>
        /// Binding query recursion depth
        /// 绑定查询递归深度
        /// </summary>
        int BindingQueryRecursionDepth { get; set; }

        XPipeline<IViewModelHandler> ViewModeHandlers { get; }

        string GetI18NText(string key, string group, string defaultValue);
    }
}
