using System;

namespace DosinisSDK.Core
{
    public interface IWindow
    {
        event Action OnShown;
        event Action OnHidden;
        event Action OnBeforeShow;
        event Action OnBeforeHide;

        internal void Init(IApp app);
        internal void Dispose();
        
        /// <summary>
        /// Meaning animating in or out and not necessarily shown or hidden
        /// </summary>
        bool Activated { get; }
        bool IsShown { get; }
        bool Initialized { get; }

        void Show();
        void Show(Action done, Action onHidden = null, Action onBeforeHide = null);
        void ForwardTo<T>(bool waitUntilHidden = false) where T : IWindow;
        void Hide();
        void Hide(Action done);
    }
}
