using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.MVVM.Components;
using UnityEngine.UI;
using UnityEngine;
using TinaX.UIKit.MVVM.Interfaces;

namespace TinaX.UIKit.MVVM.Components
{
    [AddComponentMenu("TinaX/UIKit/Binder/Button/OnClick Binder")]
    [RequireComponent(typeof(Button))]
    public class ButtonOnClickBinder : BinderBase<Button>, IEventEmitter
    {
        public Action OnEvent { get; set; }

        private void Start()
        {
            Target.onClick.AddListener(() =>
            {
                this.OnEvent?.Invoke();
            });
        }
    }
}
