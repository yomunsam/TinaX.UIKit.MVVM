using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.Services;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.MVVM.Pipeline;

namespace TinaX.UIKit.MVVM.Services
{
    public class UIKitMvvmService : IUIKitMvvmService
    {
        private ILocalizationService _localization;

        /// <summary>
        /// I18N 内置服务接口
        /// </summary>
        private ILocalizationService LocalizationService //因为本类被初始化的地方非常早，所以不能直接依赖注入，这时候ILocalizationService可能还没注册
        {
            get
            {
                if(_localization == null)
                {
                    lock (this)
                    {
                        if (_localization == null)
                            XCore.GetMainInstance().Services.TryGet(out _localization);
                    }
                }
                return _localization;
            }
        }

        /// <summary>
        /// Binding query recursion depth
        /// 绑定查询递归深度
        /// </summary>
        public int BindingQueryRecursionDepth { get; set; } = 5;

        public XPipeline<IViewModelHandler> ViewModeHandlers { get; private set; } = new XPipeline<IViewModelHandler>();

        public string GetI18NText(string key, string group, string defaultValue)
        {
            return this.LocalizationService.GetText(key, group, defaultValue);
        }

    }
}
