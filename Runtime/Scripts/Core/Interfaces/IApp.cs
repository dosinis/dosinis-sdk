using System;

namespace DosinisSDK.Core
{
    public interface IApp
    {
        event Action<bool> OnAppPaused;
        event Action<bool> OnAppFocus;
        public T GetCachedModule<T>() where T : class, IModule;
        public void RegisterModule(IModule module);
    }
}