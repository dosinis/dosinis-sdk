using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class StrictPanel<TWindow> : BasePanel where TWindow : IWindow
    {
        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            Initialized = ParentWindow is TWindow;
            if (!Initialized)
            {
                Debug.LogWarning("Current panel is designed to work only with " + typeof(TWindow).Name + " window.");
            }
        }
    }
}