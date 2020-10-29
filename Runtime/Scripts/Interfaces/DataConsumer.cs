using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit.MVVM.Interfaces
{
    /// <summary>
    /// 数据消费者
    /// </summary>
    public interface IDataConsumer { }

    public interface IDataConsumer<T> : IDataConsumer
    {
        T Value { set; }
    }

}
