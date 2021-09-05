using System;

namespace DosinisSDK.Core
{
    public interface IApp
    {
        event Action<bool> OnAppPaused;
        event Action<bool> OnAppFocus;
        event Action OnAppInitialized;
        T GetCachedModule<T>() where T : class, IModule;
        void RegisterModule(IModule module);
        ModulesRegistry ModulesRegistry { get; }
        void Restart();
    }
}
