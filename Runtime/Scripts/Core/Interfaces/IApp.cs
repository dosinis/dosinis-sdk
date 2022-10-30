using System;
using System.Threading.Tasks;
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
        T CreateModule<T>(T source = default, ModuleConfig config = null) where T : class, IModule;
        Task CreateModuleAsync<T>(T source = default, ModuleConfig config = null) where T : class, IModule, IAsyncModule;
        void Restart();
        void SwitchLoadedScene();
        void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, 
            bool switchLoadedScene = true, Action done = null);
        ITimer Timer { get; }
        CoroutineManager Coroutine { get; }
    }
}
