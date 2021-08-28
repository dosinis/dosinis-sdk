using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Module : IModule
    {
        public abstract void Init(IApp app);
    }

    public class Module<T> : Module where T : ModuleConfig
    {
        protected ModuleConfig config;

        public override void Init(IApp app)
        {
            foreach (var cfg in app.ModulesRegistry.Configs)
            {
                if (cfg.GetType() == typeof(T))
                {
                    config = cfg;
                    break;
                }
            }
        }

        protected T GetConfigAs<T>() where T : ModuleConfig
        {
            return config as T;
        }

        protected void Log(string message)
        {
            if (config && config.enableLogs)
                Debug.Log($"[{GetType().Name}] {message}");
        }

        protected void LogError(string message)
        {
            if (config && config.enableLogs)
                Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected void Warn(string message)
        {
            if (config && config.enableLogs)
                Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }
}
