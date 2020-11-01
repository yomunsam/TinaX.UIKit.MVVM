using System;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM.Interfaces;

namespace TinaX.UIKit.MVVM.BindingConverter
{
    /// <summary>
    /// 监听 绑定转换器 【可绑定属性-> 数据消费者】
    /// </summary>
    /// <typeparam name="TBindableProperty">BindableProperty 的 泛型类型</typeparam>
    /// <typeparam name="TDataConsumer">数据消费者 的 泛型类型</typeparam>
    public class ListenBindingConverter<TBindableProperty, TDataConsumer> : IDisposable
    {

        private BindableProperty<TBindableProperty> _BindableProperty;
        private IDataConsumer<TDataConsumer> _DataConsumer;

        

        private bool disposedValue;

        public ListenBindingConverter(BindableProperty<TBindableProperty> bindableProperty, IDataConsumer<TDataConsumer> dataConsumer)
        {
            if (bindableProperty == null) throw new ArgumentNullException(nameof(bindableProperty));
            if (dataConsumer == null) throw new ArgumentNullException(nameof(bindableProperty));

            _BindableProperty = bindableProperty;
            _DataConsumer = dataConsumer;

            

            _BindableProperty.ValueChanged += OnPropertyValueChanged;
        }

        private void OnPropertyValueChanged(TBindableProperty oldValue, TBindableProperty newValue)
        {
            _DataConsumer.Value = (TDataConsumer)Convert.ChangeType(newValue, typeof(TDataConsumer));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //释放托管状态(托管对象) | 释放本对象中管理的托管资源
                }

                //释放未托管的资源(未托管的对象)并替代终结器
                //将大型字段设置为 null


                //---把本对象中注册的委托给释放掉
                if (_BindableProperty != null)
                    _BindableProperty.ValueChanged -= OnPropertyValueChanged;

                _BindableProperty = null;
                _DataConsumer = null;


                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ListenBindingConverter()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
