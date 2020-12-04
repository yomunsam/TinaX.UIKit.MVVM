using System;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM.Interfaces;

namespace TinaX.UIKit.MVVM.BindingConverter
{
    /// <summary>
    /// 数据提供者 绑定转换器 - 可监听属性和数据提供者为同一类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProducerBindingSameType <T> : IDisposable
    {
        private BindableProperty<T> _BindableProperty;
        private IDataProducer<T> _DataProducer;

        private bool disposedValue;

        public ProducerBindingSameType(BindableProperty<T> bindableProperty , IDataProducer<T> dataProducer)
        {
            if (bindableProperty == null) throw new ArgumentNullException(nameof(bindableProperty));
            if (dataProducer == null) throw new ArgumentNullException(nameof(dataProducer));

            _BindableProperty = bindableProperty;
            _DataProducer = dataProducer;

            _DataProducer.OnValueChange += OnProducerValueChanged;
        }

        private void OnProducerValueChanged(T value)
        {
            _BindableProperty.Value = value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                }

                // 释放未托管的资源(未托管的对象)并替代终结器
                // 将大型字段设置为 null

                //------释放本对象注册的委托
                if (_DataProducer != null)
                    _DataProducer.OnValueChange -= OnProducerValueChanged;

                _BindableProperty = null;
                _DataProducer = null;

                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ProducerBindingSameType()
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
