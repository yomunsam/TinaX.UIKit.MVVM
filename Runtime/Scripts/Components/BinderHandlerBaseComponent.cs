using TinaX.Core.Localization;
using TinaX.UIKit.MVVM.Services;
using TinaX.Utils;
using UnityEngine;

namespace TinaX.UIKit.MVVM.Components
{
    /// <summary>
    /// 绑定器的处理器
    /// </summary>
    [DisallowMultipleComponent]
    public class BinderHandlerBaseComponent : MonoBehaviour
    {
        public DisposableGroup DisposableGroup { get; private set; } = new DisposableGroup();

        protected IXCore _Core;
        protected IUIKitMvvmService _MvvmService;

        protected virtual void Awake()
        {
            _Core = XCore.MainInstance;
            if (_Core == null)
            {
                Debug.LogError("[TinaX.UIKit.MVVM]TinaX core framework not started.");
                return;
            }

            if (!XCore.MainInstance.Services.TryGet<IUIKitMvvmService>(out _MvvmService))
            {
                Debug.LogError("[TinaX.UIKit.MVVM]" +
                    (_Core.IsCmnHans() ? "获取MVVM相关服务失败，请在TinaX核心框架中添加MVVM相关服务" :
                    "Failed to get MVVM related services. Please add MVVM services in the tinax core framework"));
                return;
            }
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void OnDestroy()
        {
            this.DisposableGroup?.Dispose();
        }

    }
}
