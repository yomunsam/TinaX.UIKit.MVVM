using TinaX.UIKit.MVVM.Exceptions;
using UnityEngine;


namespace TinaX.UIKit.MVVM.Components
{
    public class BinderBase : MonoBehaviour
    {
        [TextArea]
        public string BindingPath;

    }

    public class BinderBase<T> : BinderBase
    {
        public T Target;

        public UIPageBinderHandlerComponent ViewModeHandler { get; private set; }

        protected virtual void Awake()
        {
            if(this.Target == null)
            {
                Target = gameObject.GetComponent<T>();
            }

            if(ViewModeHandler == null)
            {
                ViewModeHandler = TryGetViewModeHandlerOrCreate();
            }
        }



        private UIPageBinderHandlerComponent TryGetViewModeHandlerOrCreate()
        {
            UIPage page = this.transform.GetComponentInParent<UIPage>();
            if(page == null)
            {
                throw new MvvmException($"Cannot found component \"UIPage\" in \"{this.gameObject.name}\"'s parents.", MvvmErrorCode.UIPageNotFoundInParent);
            }

            lock (page)
            {
                return page.gameObject.GetComponentOrAdd<UIPageBinderHandlerComponent>();
            }
        }
    }

}

