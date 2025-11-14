using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IUIManager : IModule
    {
        Camera Camera { get; }
        Canvas GetCanvas(RenderMode renderMode);
        T GetWindow<T>() where T : IWindow;
        bool TryGetWindow<T>(out T window) where T : IWindow;
        Task WaitForWindowAsync<T>() where T : IWindow;
        bool IsWindowReady<T>() where T : IWindow;
        void ShowWindow<T>(Action callBack = null, Action onHidden = null, Action onBeforeHide = null) where T : IWindow;
        void ShowWindow<T>() where T : IWindow;
        void ShowWindowWithArgs<T, TArgs>(TArgs args, Action callBack = null, Action onHidden = null, Action onBeforeHide = null)
            where T : IWindowWithArgs<TArgs>;
        void HideWindow<T>(Action callBack) where T : IWindow;
        void HideWindow<T>() where T : IWindow;
        bool IsWindowShown<T>() where T : IWindow;
        void RegisterWindow(IWindow window, bool initialize = true);
        bool AnyPopupShown();
        Task WaitForPopupsHidden();
    }
}
