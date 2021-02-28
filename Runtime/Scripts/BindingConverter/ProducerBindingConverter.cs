using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM.Interfaces;
using UnityEngine;

namespace TinaX.UIKit.MVVM.BindingConverter
{
    /// <summary>
    /// 提供者 绑定转换器 【可绑定属性-> 数据消费者】
    /// </summary>
    public class ProducerBindingConverter<TBindableProperty, TDataProducer> : IDisposable
    {
        private BindableProperty<TBindableProperty> _BindableProperty;
        private IDataProducer<TDataProducer> _DataProducer;

        private bool disposedValue;

        public ProducerBindingConverter(BindableProperty<TBindableProperty> bindableProperty, IDataProducer<TDataProducer> dataProducer)
        {
            if (bindableProperty == null) throw new ArgumentNullException(nameof(bindableProperty));
            if (dataProducer == null) throw new ArgumentNullException(nameof(dataProducer));

            _BindableProperty = bindableProperty;
            _DataProducer = dataProducer;

            _DataProducer.OnValueChange += OnProducerValueChanged;
        }

        private void OnProducerValueChanged(TDataProducer value)
        {
            try
            {
                _BindableProperty.Value = (TBindableProperty)Convert.ChangeType(value, typeof(TBindableProperty));
            }
            catch(Exception e)
            {
                if(value.GetType().IsAssignableFrom(typeof(string)))
                {
                    var _bindableType = typeof(TBindableProperty);
                    if(_bindableType.IsAssignableFrom(typeof(int)))
                    {
                        _BindableProperty.Value = (TBindableProperty)(object)0;
                        return;
                    }
                    if (_bindableType.IsAssignableFrom(typeof(long)))
                    {
                        _BindableProperty.Value = (TBindableProperty)(object)0L;
                        return;
                    }
                    if (_bindableType.IsAssignableFrom(typeof(float)))
                    {
                        _BindableProperty.Value = (TBindableProperty)(object)0f;
                        return;
                    }
                    if (_bindableType.IsAssignableFrom(typeof(double)))
                    {
                        _BindableProperty.Value = (TBindableProperty)(object)0d;
                        return;
                    }
                }
                Debug.LogError($"[TinaX.UIKit MVVM]Covert data exception: " + e.Message);
            }
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

                //---把本对象中注册的委托给释放掉
                if (_DataProducer != null)
                    _DataProducer.OnValueChange -= OnProducerValueChanged;

                _DataProducer = null;
                _BindableProperty = null;

                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ProducerBindingConverter()
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
