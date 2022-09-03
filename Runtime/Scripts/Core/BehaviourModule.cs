using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class BehaviourModule : MonoBehaviour, IBehaviourModule
    {
        [SerializeField] protected ModuleConfig mainConfig;

        void IModule.Init(IApp app, ModuleConfig config)
        {
            if (config)
                mainConfig = config;

            OnInit(app);
        }

        protected abstract void OnInit(IApp app);

        public T GetConfigAs<T>() where T : ModuleConfig
        {
            return mainConfig as T;
        }

        protected void Log(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.Log($"[{GetType().Name}] {message}");
        }

        protected void LogError(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogError($"[{GetType().Name}] {message}");
        }

        protected void Warn(string message)
        {
            if (mainConfig == null || mainConfig && mainConfig.enableLogs)
                Debug.LogWarning($"[{GetType().Name}] {message}");
        }
    }
}
