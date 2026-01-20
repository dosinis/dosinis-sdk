using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class StrictPanel<TWindow> : BasePanel where TWindow : IWindow
    {
        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            initialized = parentWindow is TWindow;
            if (!initialized)
            {
                Debug.LogWarning("Current panel is designed to work only with " + typeof(TWindow).Name + " window.");
            }
        }
    }
}