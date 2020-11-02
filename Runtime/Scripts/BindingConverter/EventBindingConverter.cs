using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.MVVM.Interfaces;

namespace TinaX.UIKit.MVVM.BindingConverter
{
    /// <summary>
    /// 处理IEventEmitter的
    /// </summary>
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class EventBindingConverter : IDisposable
    {
        private IEventEmitter _EventEmitter;
        private Action _Action;

        public EventBindingConverter(IEventEmitter eventEmitter, Action action)
        {
            _EventEmitter = eventEmitter;
            _Action = action;
            _EventEmitter.OnEvent += _Action;
        }

        public void Dispose()
        {
            if(_EventEmitter != null && _Action != null)
            {
                _EventEmitter.OnEvent -= _Action;
            }
        }
    }
#pragma warning restore CA1063 // Implement IDisposable Correctly

}
