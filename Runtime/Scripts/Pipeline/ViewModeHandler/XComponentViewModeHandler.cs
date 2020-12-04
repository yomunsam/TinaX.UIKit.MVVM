using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TinaX.Core.Localization;
using TinaX.UIKit.Components;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM.BindingConverter;
using TinaX.UIKit.MVVM.Components;
using TinaX.UIKit.MVVM.Interfaces;
using TinaX.UIKit.MVVM.Services;
using TinaX.XComponent;
using UnityEngine;

namespace TinaX.UIKit.MVVM.Pipeline
{
    public class XComponentViewModeHandler : IViewModelHandler
    {
        


        public IViewModelHandler Handler => this;

        public bool HandleViewModel(BinderHandlerBaseComponent handlerComponent, IUIKitMvvmService mvvmService)
        {
            var xcomponent = handlerComponent.gameObject.GetComponent<TinaX.XComponent.XComponent>();
            if(xcomponent == null)
            {
                return false; //当前handler不处理该UI
            }

            var xbehaviour = xcomponent.Behaviour;

            //获取所有Binder组件
            var binders = handlerComponent.gameObject.GetComponentsInChildren<BinderBase>();
            //Debug.Log("获取到Binder组件数：" + binders.Length);



            //反射出所有绑定数据
            Dictionary<string, object> _dict_BindableProperties = new Dictionary<string, object>(); //string:key , object: bindableProperty
            this.TraverseBindingDataRecursion(ref _dict_BindableProperties, xbehaviour, mvvmService.BindingQueryRecursionDepth, 1, true);
            //Debug.Log("反射绑定数据完成");

            //反射所有Command
            Dictionary<string, Delegate> _dict_commands = new Dictionary<string, Delegate>();
            this.GetCommands(ref _dict_commands, xbehaviour);


            //处理Binder
            foreach(var binder in binders)
            {
                //处理数据消费者
                if(binder is IDataConsumer)
                {
                    this.HandleListenTo(binder, ref handlerComponent, ref _dict_BindableProperties);
                }

                //处理数据提供者
                if(binder is IDataProducer)
                {
                    this.HandleProducer(binder, ref handlerComponent, ref _dict_BindableProperties);
                }

                //处理事件发射器
                if(binder is IEventEmitter)
                {
                    this.HandleEventEmitter(binder, ref handlerComponent, ref _dict_commands);
                }

            }

            _dict_BindableProperties.Clear();
            _dict_BindableProperties = null;
            _dict_commands.Clear();
            _dict_commands = null;

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
            if (resultDict.Any(kv => kv.Value == currentData))
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

        /// <summary>
        /// 获取Commands
        /// </summary>
        /// <param name="resultDict">存放结果的字典</param>
        /// <param name="targetObj">在某个对象里寻找</param>
        /// <param name="ParentPath">存入字典的时候要不要拼接父级Path</param>
        private void GetCommands(ref Dictionary<string,Delegate> resultDict, object targetObj, string ParentPath = null)
        {
            var targetType = targetObj.GetType();
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            if(methods != null && methods.Length > 0)
            {
                foreach(var method in methods)
                {
                    var attribute = method.GetCustomAttribute<CommandAttribute>(true);
                    if (attribute == null)
                        continue;
                    var parameters = method.GetParameters();
                    if (parameters.Length != 0)
                        continue;
                    if (method.ReturnType != typeof(void))
                        continue;

                    Delegate dCommand = Delegate.CreateDelegate(typeof(Action), targetObj, method);
                    string pathName = ParentPath == null ? method.Name : $"{ParentPath}.{method.Name}";
                    resultDict.AddOrOverride(pathName, dCommand);
                }
            }
        }

        private void HandleListenTo(BinderBase binder, ref BinderHandlerBaseComponent handlerComponent, ref Dictionary<string, object> dict_bindableProperties)
        {
            string bindingPath = binder.BindingPath;
            if (dict_bindableProperties.TryGetValue(bindingPath, out object _bindable)) //查找Path
            {
                Type bindableType = _bindable.GetType();
                //BindableProperty 泛型类型
                Type bindablePropertyGenericsType = bindableType.GetGenericArguments().FirstOrDefault();
                if (bindablePropertyGenericsType == null)
                {
                    if (XCore.MainInstance?.IsCmnHans() ?? false)
                    {
                        //中文报错：
                        Debug.LogError($"[TinaX.UIKit.MVVM]绑定路径\"{bindingPath}\"是一个无效的BindableProperty");
                    }
                    else
                    {
                        Debug.LogError($"[TinaX.UIKit.MVVM]Binding path \"{bindingPath}\"is invalid BindableProperty");
                    }
                }

                //数据消费者的泛型
                Type binderType = ((IDataConsumer)binder).GetType();
                var binderGenericsType = this.GetDataConsumerType(binderType);

                if(binderGenericsType == bindablePropertyGenericsType)
                {
                    //反射生成一个同类型的监听绑定转换器 
                    var converterType = typeof(ListenBindingSameType<>).MakeGenericType(new Type[] { binderGenericsType });
                    var converter = XCore.MainInstance.CreateInstance(converterType, new object[] { _bindable, binder });
                    //把生成的绑定转换器交给ViewHandlerComponent来管理生命周期。
                    handlerComponent.DisposableGroup.Register((IDisposable)converter);
                }
                else
                {
                    //反射生成一个“可绑定属性”与“数据消费者” 类型不一致的绑定转换器
                    var converterType = typeof(ListenBindingConverter<,>).MakeGenericType(new Type[] { bindablePropertyGenericsType, binderGenericsType });
                    var converter = XCore.MainInstance.CreateInstance(converterType, new object[] { _bindable, binder });
                    //把生成的绑定转换器交给ViewHandlerComponent来管理生命周期
                    handlerComponent.DisposableGroup.Register((IDisposable)converter);
                }
            }
        }

        /// <summary>
        /// 处理事件发射器
        /// </summary>
        private void HandleEventEmitter(BinderBase binder, ref BinderHandlerBaseComponent handlerComponent, ref Dictionary<string, Delegate> dict_commands)
        {
            string bindingPath = binder.BindingPath;
            if (dict_commands.TryGetValue(bindingPath, out Delegate _delegate)) //查找Path
            {
                //执行绑定
                EventBindingConverter converter = new EventBindingConverter((IEventEmitter)binder, (Action)_delegate);
                handlerComponent.DisposableGroup.Register(converter);
            }
        }

        private void HandleProducer(BinderBase binder , ref BinderHandlerBaseComponent handlerComponent, ref Dictionary<string,object> dict_bindableProperties)
        {
            string bindingPath = binder.BindingPath;
            if (dict_bindableProperties.TryGetValue(bindingPath, out object _bindable)) //查找Path
            {
                Type bindableType = _bindable.GetType();
                //BindableProperty 泛型类型
                Type bindablePropertyGenericsType = bindableType.GetGenericArguments().FirstOrDefault();
                if (bindablePropertyGenericsType == null)
                {
                    if (XCore.MainInstance?.IsCmnHans() ?? false)
                    {
                        //中文报错：
                        Debug.LogError($"[TinaX.UIKit.MVVM]绑定路径\"{bindingPath}\"是一个无效的BindableProperty");
                    }
                    else
                    {
                        Debug.LogError($"[TinaX.UIKit.MVVM]Binding path \"{bindingPath}\"is invalid BindableProperty");
                    }
                }


                //数据提供者的泛型类型
                Type binderType = ((IDataProducer)binder).GetType();
                var binderGenericsType = this.GetDataProducerType(binderType);

                if (binderGenericsType == bindablePropertyGenericsType)
                {
                    //反射生成一个同类型的监听绑定转换器 
                    var converterType = typeof(ProducerBindingSameType<>).MakeGenericType(new Type[] { binderGenericsType });
                    var converter = XCore.MainInstance.CreateInstance(converterType, new object[] { _bindable, binder });
                    //把生成的绑定转换器交给ViewHandlerComponent来管理生命周期。
                    handlerComponent.DisposableGroup.Register((IDisposable)converter);
                }
                else
                {
                    //反射生成一个“可绑定属性”与“数据消费者” 类型不一致的绑定转换器
                    var converterType = typeof(ProducerBindingConverter<,>).MakeGenericType(new Type[] { bindablePropertyGenericsType, binderGenericsType });
                    var converter = XCore.MainInstance.CreateInstance(converterType, new object[] { _bindable, binder });
                    //把生成的绑定转换器交给ViewHandlerComponent来管理生命周期
                    handlerComponent.DisposableGroup.Register((IDisposable)converter);
                }
            }
        }

        /// <summary>
        /// 获取数据消费者的泛型类型
        /// </summary>
        /// <returns></returns>
        private Type GetDataConsumerType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Length < 1)
                return null;

            foreach(var i in interfaces)
            {
                if (i.IsGenericType)
                {
                    if(typeof(IDataConsumer).IsAssignableFrom(i))
                    {
                        return i.GetGenericArguments().FirstOrDefault();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取数据提供者的泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type GetDataProducerType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Length < 1)
                return null;

            foreach (var i in interfaces)
            {
                if (i.IsGenericType)
                {
                    if (typeof(IDataProducer).IsAssignableFrom(i))
                    {
                        return i.GetGenericArguments().FirstOrDefault();
                    }
                }
            }

            return null;
        }

    }
}
