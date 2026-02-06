using DosinisSDK.Core;
using UnityEngine.EventSystems;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationCancel : UINavigationBase
    {
        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            if (Target.activeInHierarchy)
            {
                navigationController.RegisterCancellationElement(this);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            navigationController?.RegisterCancellationElement(this);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            navigationController?.UnregisterCancellationElement();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            navigationController.UnregisterCancellationElement();
        }

        protected override void OnCancel()
        {
            EventSystem.current.SetSelectedGameObject(Target);
            OnSubmit();
        }
    }
}