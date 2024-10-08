using System;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Module : IModule, IDisposable
    {
        private ModuleConfig mainConfig;
        
        void IModule.Init(IApp app, ModuleConfig config)
        {
            if (config)
                mainConfig = config;
        
            OnInit(app);
        }

        protected abstract void OnInit(IApp app);
        
        protected T GetConfigAs<T>() where T : ModuleConfig
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

        /// <summary>
        /// Used to dispose resources for stuff that is out of the scope of the framework modules lifecycle
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        
        void IDisposable.Dispose()
        {
            OnDispose();
        }
    }
}
