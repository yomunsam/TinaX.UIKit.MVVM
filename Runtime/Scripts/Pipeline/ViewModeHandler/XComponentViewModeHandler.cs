using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TinaX.UIKit.Components;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM.Components;
using TinaX.UIKit.MVVM.Interfaces;
using TinaX.UIKit.MVVM.Services;
using TinaX.XComponent;
using UnityEngine;

namespace TinaX.UIKit.MVVM.Pipeline
{
    public class XComponentViewModeHandler : IViewModelHandler
    {
        

        private Dictionary<string, object> _Dict_BindableProperties = new Dictionary<string, object>();

        public IViewModelHandler Handler => this;

        public bool HandleViewModel(ViewModeHandlerComponent handlerComponent, IUIKitMvvmService mvvmService)
        {
            var xcomponent = handlerComponent.gameObject.GetComponent<TinaX.XComponent.XComponent>();
            if(xcomponent == null)
            {
                return false; //当前handler不处理该UI
            }

            var xbehaviour = xcomponent.Behaviour;

            //获取所有Binder组件
            var binders = handlerComponent.gameObject.GetComponentsInChildren<BinderBase>();
            Debug.Log("获取到Binder组件数：" + binders.Length);

            

            //反射出所有绑定数据
            this.TraverseBindingDataRecursion(ref _Dict_BindableProperties, xbehaviour, mvvmService.BindingQueryRecursionDepth, 1, true);
            Debug.Log("反射绑定数据完成");

            //处理Binder
            foreach(var binder in binders)
            {
                //处理数据消费者
                if(binder is IDataConsumer)
                {
                    this.HandleListenTo(binder);
                }
            }

            return true;
        }


        /// <summary>
        /// 遍历绑定数据【递归】
        /// </summary>
        /// <param name="resultDict">绑定数据存储结果</param>
        /// <param name="currentData">当前数据</param>
        /// <param name="maxDepth">最大遍历深度</param>
        /// <param name="currentDepth">当前深度</param>
        /// <param name="basePath">当前层级的基础path</param>
        /// <param name="requireBindableAttribute">仅反射打了"Bindable" attribute的属性</param>
        private void TraverseBindingDataRecursion(ref Dictionary<string, object> resultDict, object currentData, int maxDepth, int currentDepth,bool requireBindableAttribute = false, string currentPath = null)
        {
            //判定data对象是否已存在
            if (_Dict_BindableProperties.Any(kv => kv.Value == currentData))
                return;
            //判定层级深度
            if (currentDepth > maxDepth)
                return;

            var data_type = currentData.GetType();
            if (data_type.HasImplementedRawGeneric(typeof(BindableProperty<>)))
            {
                //是BindableProperty<>类，把它添加到字典
                if (!resultDict.ContainsKey(currentPath))
                {
                    resultDict.Add(currentPath, currentData);
                }
            }
            else
            {
                //是一个非“BindableProperty<>”的类，往下一层级作递归
                var bindable_properties = data_type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (requireBindableAttribute)
                    bindable_properties = bindable_properties.Where(p => p.IsDefined(typeof(BindableAttribute))).ToArray();

                if(bindable_properties.Length > 0)
                {
                    foreach(var p in bindable_properties)
                    {
                        var p_data = p.GetValue(currentData);
                        if(p_data != null)
                        {
                            this.TraverseBindingDataRecursion(ref resultDict, p_data, maxDepth, currentDepth + 1, false, 
                                (currentPath == null ? p.Name : $"{currentPath}.{p.Name}"));
                        }
                    }
                }

            }
        }


        private void HandleListenTo(BinderBase binder)
        {
            string bindingPath = binder.BindingPath;
            if(_Dict_BindableProperties.TryGetValue(bindingPath, out object _bindable))
            {
                var valueChangedDelegate = _bindable.GetType().GetField("ValueChanged");
                Debug.Log("测试动态类型给委托赋值");
                dynamic bindable_dynamic = (dynamic)_bindable;

            }
        }

    }
}
