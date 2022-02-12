using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public interface IApp
    {
        event Action<bool> OnAppPaused;
        event Action<bool> OnAppFocus;
        event Action OnAppQuit;
        T GetModule<T>() where T : class, IModule;
        void RegisterModule(IModule module, ModuleConfig config = null);
        void CreateBehaviourModule<T>(T source = null) where T : BehaviourModule;
        void Restart();
        void LoadScene(int sceneIndex, Action done = null, LoadSceneMode mode = LoadSceneMode.Single);
        ITimer Timer { get; }
        ICoroutineManager Coroutine { get; }
        ISceneManager SceneManager { get; }
    }
}
