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
        bool IsPopup { get; }

        void Show();
        void ShowImmediately();
        void ShowImmediately(Action done, Action onHidden = null, Action onBeforeHide = null);
        
        void Show(Action done, Action onHidden = null, Action onBeforeHide = null);
        void ForwardTo<T>(bool waitUntilHidden = false) where T : IWindow;
        void Hide();
        void Hide(Action done);
        void HideImmediately();
        void Refresh();
        void RegisterWidget(Widget widget);
        void ClearHideCallbacks();
    }
}
