using System;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public interface IApp
    {
        event Action<bool> OnAppPaused;
        event Action<bool> OnAppFocus;
        event Action OnAppQuit;
        event Action OnAppRestart;
        T GetModule<T>() where T : class, IModule;
        bool TryGetModule<T>(out T module) where T : class, IModule;
        bool IsModuleReady<T>() where T : class, IModule;
        void RegisterModule(IModule module, ModuleConfig config = null);
        void CreateBehaviourModule<T>(T source = null, ModuleConfig config = null) where T : BehaviourModule;
        void Restart();
        void SwitchLoadedScene();
        void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, 
            bool switchLoadedScene = true, Action done = null);
        ITimer Timer { get; }
        CoroutineManager Coroutine { get; }
    }
}
