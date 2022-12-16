using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IUIManager : IModule
    {
        Camera Camera { get; }
        T GetWindow<T>() where T : IWindow;
        bool IsWindowReady<T>() where T : IWindow;
        void ShowWindow<T>(Action callBack = null, Action onHidden = null) where T : IWindow;
        void ShowWindowWithArgs<T, TArgs>(TArgs args, Action callBack = null, Action onHidden = null)
            where T : IWindowWithArgs<TArgs>;
        void HideWindow<T>(Action callBack = null) where T : IWindow;
    }
}
