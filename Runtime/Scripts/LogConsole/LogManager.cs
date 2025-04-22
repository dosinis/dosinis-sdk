using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK
{
    public class LogManager : Module, IProcessable
    {
        private LogConsole logConsole;

        protected override void OnInit(IApp app)
        {
            LogConfig logConfig = GetConfigAs<LogConfig>();

            logConsole = CreateLogConsole(app, logConfig);
        }

        public void Process(in float delta)
        {
            logConsole.Process(delta);
        }

        protected override void OnDispose()
        {
            logConsole.Dispose();
        }

        private LogConsole CreateLogConsole(IApp app, LogConfig logConfig)
        {
            var logConsoleInstance = Object.Instantiate(logConfig.GetLogConsolePrefab);
            logConsoleInstance.OnInit(app, logConfig.GetGoogleScriptUrl);

            Object.DontDestroyOnLoad(logConsoleInstance.gameObject);
            
            return logConsoleInstance;
        }
    }
}