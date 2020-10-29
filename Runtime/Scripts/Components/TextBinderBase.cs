using TinaX.UIKit.MVVM.Interfaces;

namespace TinaX.UIKit.MVVM.Components
{
    /// <summary>
    /// UI Text绑定类型的基类，额外提供了字符串拼接，I18N等功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TextBinderBase<T> : BinderBase<T>, IDataConsumer<string>
    {
        [System.Serializable]
        public enum FormatMode : byte
        {
            Normal  = 0,
            TextMessage = 1,
            I18NValue = 2,
            Custom = 3,
        }

        public delegate void SetValueDelegate(string value, TextBinderBase<T> binder);

        public FormatMode ValueFormatMode;
        public string TextMessage;
        public string I18NGroup;
        public string I18NKey;

        public SetValueDelegate SetValue { get; set; }

        public virtual string Value { set => SetValue?.Invoke(value, this); }

        protected override void Awake()
        {
            base.Awake();
            switch (ValueFormatMode)
            {
                default:
                case FormatMode.Normal:
                case FormatMode.Custom:
                    this.SetValue = SetValueNormal;
                    break;
                case FormatMode.TextMessage:
                    this.SetValue = SetValueTextMessage;
                    break;
                case FormatMode.I18NValue:
                    this.SetValue = SetValueI18N;
                    break;

            }
        }

        protected virtual void SetValueNormal(string value, TextBinderBase<T> binder) { }

        protected virtual void SetValueTextMessage(string value, TextBinderBase<T> binder) { }

        protected virtual void SetValueI18N(string value, TextBinderBase<T> binder) { }

    }
}
