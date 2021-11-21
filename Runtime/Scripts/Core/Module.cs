using UnityEngine;

namespace DosinisSDK.Core
{
    public class Module : IModule
    {
        protected ModuleConfig mainConfig;

        public virtual void Init(IApp app, ModuleConfig config = null)
        {
            if (config)
                mainConfig = config;
        }

        public T GetConfigAs<T>() where T : ModuleConfig
        {
            return mainConfig as T;
        }

        protected virtual void Log(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.Log($"[{GetType().Name}] {message}");
        }

        protected virtual void LogError(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected virtual void Warn(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }
}
