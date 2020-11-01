using UnityEngine.UI;
using UnityEngine;
using TinaX.UIKit.MVVM.Interfaces;
using TinaX.UIKit.MVVM.Const;

namespace TinaX.UIKit.MVVM.Components
{
    [AddComponentMenu("TinaX/UIKit/Binder/Text/Text Binder")]
    [RequireComponent(typeof(Text))]
    public class TextTextBinder : TextBinderBase<Text>
    {
        protected override void SetValueNormal(string value, TextBinderBase<Text> binder)
        {
            this.Target.text = value;
        }

        protected override void SetValueTextMessage(string value, TextBinderBase<Text> binder)
        {
            this.Target.text = this.TextMessage.Replace(UIKitMvvmConst.TextMessageValueSign, value);
        }
    }
}
