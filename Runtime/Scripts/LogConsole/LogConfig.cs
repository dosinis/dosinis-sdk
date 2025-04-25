using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.LogConsole
{
    [CreateAssetMenu(fileName = "LogConfig", menuName = "DosinisSDK/Configs/LogConfig")]
    public class LogConfig : ModuleConfig
    {
        [SerializeField] private bool isOpenByKey;
        [SerializeField] private string googleScriptUrl;

        public bool IsOpenByKey => isOpenByKey;
        public string GetGoogleScriptUrl => googleScriptUrl;
    }
}