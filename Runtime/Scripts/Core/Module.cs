using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Module : IModule
    {
        public abstract void Init(IApp app);

        protected virtual void Log(string message)
        {
            Debug.Log($"[{GetType().Name}] {message}");
        }

        protected virtual void LogError(string message)
        {
            Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected virtual void Warn(string message)
        {
            Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }

    public class Module<T> : Module where T : ModuleConfig
    {
        protected ModuleConfig mainConfig;

        public override void Init(IApp app)
        {
            foreach (var cfg in app.ModulesRegistry.Configs)
            {
                if (cfg.GetType() == typeof(T))
                {
                    mainConfig = cfg;
                    break;
                }
            }
        }

        protected T GetConfigAs<T>() where T : ModuleConfig
        {
            return mainConfig as T;
        }

        protected override void Log(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.Log($"[{GetType().Name}] {message}");
        }

        protected override void LogError(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected override void Warn(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }
}
