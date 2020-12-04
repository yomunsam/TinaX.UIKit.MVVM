namespace TinaX.UIKit.MVVM.Interfaces
{
    /// <summary>
    /// 数据消费者
    /// </summary>
    public interface IDataConsumer { }

    /// <summary>
    /// 数据消费者 泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataConsumer<T> : IDataConsumer
    {
        T Value { set; }
    }

}
