using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IUIManager : IModule
    {
        Camera Camera { get; }
        T GetWindow<T>() where T : Window;
        void ShowWindow<T>(Action callBack = null, Action onHidden = null) where T : Window;
        void ShowWindowWithArgs<T, TArgs>(TArgs args, Action callBack = null, Action onHidden = null)
            where T : WindowWithArgs<TArgs>;
        void HideWindow<T>(Action callBack = null) where T : Window;
    }
}
