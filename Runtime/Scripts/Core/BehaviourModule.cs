using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class BehaviourModule : MonoBehaviour, IBehaviourModule
    {
        [SerializeField] private int initOrder = 0;
        [SerializeField] protected ModuleConfig config;

        public int InitOrder => initOrder;

        public abstract void Init(IApp app);

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
