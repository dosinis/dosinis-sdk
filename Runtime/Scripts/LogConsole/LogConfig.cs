using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK
{
    [CreateAssetMenu(fileName = "LogConfig", menuName = "DosinisSDK/Configs/LogConfig")]
    public class LogConfig : ModuleConfig
    {
        [SerializeField] private LogConsole logConsolePrefab;
        [SerializeField] private string googleScriptUrl;

        public LogConsole GetLogConsolePrefab => logConsolePrefab;
        public string GetGoogleScriptUrl => googleScriptUrl;
    }
}