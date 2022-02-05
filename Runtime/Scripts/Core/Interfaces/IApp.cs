using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IApp
    {
        event Action<bool> OnAppPaused;
        event Action<bool> OnAppFocus;
        event Action OnAppQuit;
        T GetModule<T>() where T : class, IModule;
        void RegisterModule(IModule module, ModuleConfig config = null);
        void CreateBehaviourModule<T>(GameObject source = null) where T : BehaviourModule;
        void Restart();
        ITimer Timer { get; }
        ICoroutineManager Coroutine { get; }
        ISceneManager SceneManager { get; }
    }
}
