using DosinisSDK.Core;
using UnityEngine.EventSystems;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationCancel : UINavigationBase
    {
        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            if (IsEnabled)
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
            navigationController?.UnregisterCancellationElement(this);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            navigationController.UnregisterCancellationElement(this);
        }

        protected override void OnCancel()
        {
            if(!IsActiveNavigation) return;
            EventSystem.current.SetSelectedGameObject(Target);
            OnSubmit();
        }
    }
}