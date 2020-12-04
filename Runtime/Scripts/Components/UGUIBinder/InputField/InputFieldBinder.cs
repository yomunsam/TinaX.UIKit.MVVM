using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.MVVM.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.MVVM.Components
{
    [AddComponentMenu("TinaX/UIKit/Binder/Input Field/Input Binder")]
    [RequireComponent(typeof(InputField))]
    public class InputFieldBinder : BinderBase<InputField>, IDataConsumer<string>, IDataProducer<string>
    {
        public string Value { set => Target.text = value.IsNullOrEmpty() ? "" : value; }
        public Action<string> OnValueChange { get; set; }

        private void Start()
        {
            Target.onValueChanged.AddListener(value =>
            {
                OnValueChange?.Invoke(value);
            });
        }
    }
}
