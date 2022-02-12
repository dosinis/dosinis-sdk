using UnityEngine;

namespace DosinisSDK.Core
{
    public class Widget : MonoBehaviour
    {
        protected Window parentWindow;

        public void Init(Window parentWindow)
        {
            this.parentWindow = parentWindow;
            parentWindow.OnShown += OnWindowShown;
            parentWindow.OnHidden += OnWindowHidden;
            parentWindow.OnBeforeShow += OnWindowBeforeShow;
            parentWindow.OnBeforeHide += OnWindowBeforeHide;

            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnWindowBeforeShow()
        {
        }

        protected virtual void OnWindowBeforeHide()
        {
        }

        protected virtual void OnWindowShown()
        {
        }

        protected virtual void OnWindowHidden()
        {
        }
    }
}