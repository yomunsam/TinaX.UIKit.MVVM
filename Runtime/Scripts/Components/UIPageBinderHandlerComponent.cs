using TinaX.Core.Localization;
using TinaX.UIKit.MVVM.Pipeline;
using TinaX.UIKit.MVVM.Services;
using TinaX.Utils;
using UnityEngine;

namespace TinaX.UIKit.MVVM.Components
{
    /// <summary>
    /// UGUI UIPage Binder Handler
    /// （该组件会被创建在（UIPage同级）根GameObject上）
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIPage))]
    public class UIPageBinderHandlerComponent : BinderHandlerBaseComponent
    {
        private IViewModelHandler _ViewModelHandler;




        protected override void Start()
        {
            if (_MvvmService == null)
                return;
            //处理数据绑定
            HandleDataBinding();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void HandleDataBinding()
        {
            _MvvmService.ViewModeHandlers.Start(handler =>
            {
                bool _break = handler.Handler.HandleViewModel(this, _MvvmService);
                if (_break)
                {
                    _ViewModelHandler = handler.Handler;
                    //Debug.Log("命中ViewModelHandler:" + _ViewModelHandler.ToString());
                    return false;
                }
                else
                    return true;
            });
        }
    }
}
