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
        T CreateModule<T>(T source = default, ModuleConfig config = null) where T : class, IModule;
        Task CreateModuleAsync<T>(T source = default, ModuleConfig config = null) where T : class, IModule, IAsyncModule;
        void Restart();
    }
}
