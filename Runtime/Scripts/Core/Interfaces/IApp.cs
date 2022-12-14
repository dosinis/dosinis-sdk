using System;
using System.Threading.Tasks;

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
        void Restart();
        
        // Core Modules
        
        public ICoroutineManager Coroutine => GetModule<ICoroutineManager>();
        public IDataManager DataManager => GetModule<IDataManager>();
        public IClock Clock => GetModule<IClock>();
        public ITimer Timer => GetModule<ITimer>();
        public ISceneManager SceneManager => GetModule<ISceneManager>();
        public IUIManager UIManager => GetModule<IUIManager>();
    }

    public interface IModuleFactory
    {
        T CreateModule<T>(T source = default, ModuleConfig config = null) where T : class, IModule;
        Task CreateModuleAsync<T>(T source = default, ModuleConfig config = null) where T : class, IAsyncModule;
    }
}
