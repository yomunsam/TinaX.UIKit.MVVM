using TinaX.Core.Localization;
using TinaX.UIKit.MVVM.Pipeline;
using TinaX.UIKit.MVVM.Services;
using TinaX.Utils;
using UnityEngine;

namespace TinaX.UIKit.MVVM.Components
{
    /// <summary>
    /// ViewMode Handler
    /// （该组件会被创建在（UIPage同级）根GameObject上）
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIPage))]
    public class ViewModeHandlerComponent : MonoBehaviour
    {
        public DisposableGroup DisposableGroup { get; private set; } = new DisposableGroup();

        private IUIKitMvvmService _MvvmService;
        private IViewModelHandler _ViewModelHandler;

        private void Awake()
        {
            var core = XCore.GetMainInstance();
            if(core == null)
            {
                Debug.LogError("[TinaX.UIKit.MVVM]TinaX core framework not started.");
                return;
            }
            if(!XCore.MainInstance.Services.TryGet<IUIKitMvvmService>(out _MvvmService))
            {
                Debug.LogError("[TinaX.UIKit.MVVM]" + 
                    (core.IsCmnHans() ? "获取MVVM相关服务失败，请在TinaX核心框架中添加MVVM相关服务" :
                    "Failed to get MVVM related services. Please add MVVM services in the tinax core framework"));
                return;
            }
        }


        private void Start()
        {
            if (_MvvmService == null)
                return;
            //处理数据绑定
            HandleDataBinding();
        }


        private void OnDestroy()
        {
            this.DisposableGroup?.Dispose();
        }

        private void HandleDataBinding()
        {
            _MvvmService.ViewModeHandlers.Start(handler =>
            {
                bool _break = handler.Handler.HandleViewModel(this, _MvvmService);
                if (_break)
                {
                    _ViewModelHandler = handler.Handler;
                    Debug.Log("命中ViewModelHandler:" + _ViewModelHandler.ToString());
                    return false;
                }
                else
                    return true;
            });
        }
    }
}
