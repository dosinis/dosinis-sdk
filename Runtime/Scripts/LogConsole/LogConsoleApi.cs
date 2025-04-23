using System.Threading.Tasks;
using DosinisSDK.Rest;
using UnityEngine;

namespace DosinisSDK.LogConsole
{
    public class LogConsoleApi
    {
        private IRestManager restManager;
        private string googleScriptUrl;

        public LogConsoleApi(IRestManager restManager, string googleScriptUrl)
        {
            this.googleScriptUrl = googleScriptUrl;
            this.restManager = restManager;
        }
        
        public async Task PostDataAsync(string log, string description)
        {
            try
            {
                LogData logData = new LogData()
                {
                    phone = SystemInfo.deviceModel,
                    buildVersion = $"v{Application.version}",
                    ram = SystemInfo.systemMemorySize.ToString(),
                    error = log,
                    description = description,
                };

                await restManager.PostAsync<Response<object>>(googleScriptUrl,
                    logData, new Header("Content-Type", "application/json"));
            }
            catch
            {
                throw;
            }
        }
    }
}