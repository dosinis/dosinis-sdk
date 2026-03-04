using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IUIManager : IModule
    {
        Camera Camera { get; }
        Canvas GetCanvas(CanvasType canvasType);
        T GetWindow<T>() where T : IWindow;
        T GetOrCreateWindow<T>(CanvasType canvas = CanvasType.None) where T : IWindow;
        T GetOrCreateReadyWindow<T>(CanvasType canvas) where T : IWindow;
        bool TryGetWindow<T>(out T window) where T : IWindow;
        Task WaitForWindowAsync<T>() where T : IWindow;
        bool IsWindowReady<T>() where T : IWindow;
        void ShowWindow<T>(Action shown = null, Action onHidden = null, 
            Action onBeforeHide = null, CanvasType canvas = CanvasType.None) where T : IWindow;
        void ShowWindow<T>(CanvasType canvas) where T : IWindow;
        void ShowWindow<T>() where T : IWindow;
        void ShowWindowImmediately<T>(Action onHidden = null, Action onBeforeHide = null, 
            CanvasType canvas = CanvasType.None) where T : IWindow;
        void ShowWindowWithArgs<T, TArgs>(TArgs args, Action shown = null, 
            Action onHidden = null, Action onBeforeHide = null, CanvasType canvas = CanvasType.None) where T : IWindowWithArgs<TArgs>;
        void ShowWindowImmediatelyWithArgs<T, TArgs>(TArgs args, Action onHidden = null,
            Action onBeforeHide = null, CanvasType canvas = CanvasType.None) where T : IWindowWithArgs<TArgs>;
        void HideWindow<T>(Action hidden) where T : IWindow;
        void HideWindow<T>() where T : IWindow;
        void HideImmediately<T>() where T : IWindow;
        bool IsWindowShown<T>() where T : IWindow;
        void RegisterWindow(IWindow window, bool initialize = true);
        Task<T> CreateWindowAsync<T>(CanvasType canvas, AssetLink link = null) where T : IWindow;
        T CreateWindow<T>(CanvasType canvas, AssetLink link = null) where T : IWindow;
    }
}
