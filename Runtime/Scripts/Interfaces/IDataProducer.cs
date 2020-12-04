using System;

namespace TinaX.UIKit.MVVM.Interfaces
{
    /// <summary>
    /// 数据提供者
    /// </summary>
    public interface IDataProducer { }

    /// <summary>
    /// 泛型 数据提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataProducer<T> : IDataProducer
    {
        Action<T> OnValueChange { get; set; }
    }

}
