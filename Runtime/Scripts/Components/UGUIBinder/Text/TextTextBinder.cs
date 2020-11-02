using UnityEngine.UI;
using UnityEngine;
using TinaX.UIKit.MVVM.Interfaces;
using TinaX.UIKit.MVVM.Const;
using TinaX.UIKit.MVVM.Services;

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

        protected override void SetValueI18N(string value, TextBinderBase<Text> binder)
        {
            string i18nText = XCore.GetMainInstance().Services.Get<IUIKitMvvmService>().GetI18NText(this.I18NKey, this.I18NGroup, value);
            this.Target.text = i18nText.Replace(UIKitMvvmConst.TextMessageValueSign, value);
        }
    }
}
