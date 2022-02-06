using System;

namespace DosinisSDK.Core
{
    public interface IUIManager : IBehaviourModule, IProcessable
    {
        T GetWindow<T>() where T : Window;
        void ShowWindow<T>(Action callBack = null) where T : Window;
        void HideWindow<T>(Action callBack = null) where T : Window;
    }
}

