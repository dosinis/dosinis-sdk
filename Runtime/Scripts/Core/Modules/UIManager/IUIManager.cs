using DosinisSDK.Core;
using System;

public interface IUIManager : IBehaviourModule
{
    T GetWindow<T>() where T : Window;
    void ShowWindow<T>(Action callBack = null) where T : Window;
    void HideWindow<T>(Action callBack = null) where T : Window;
}
