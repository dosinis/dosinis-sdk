using System;
using DosinisSDK.Core;
using DosinisSDK.Rest;
using TMPro;
using UnityEngine;

namespace DosinisSDK
{
    public class SendLogWidget : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button exitButton;
        
        private IRestManager restManager;
        private LogData logData;
        private string log;
        private string googleScriptUrl;

        public void Init(IRestManager restManager, string googleScriptUrl)
        {
            this.googleScriptUrl = googleScriptUrl;
            this.restManager = restManager;
            
            gameObject.SetActive(false);
        }
        
        public void Show(string log)
        {
            gameObject.SetActive(true);
            
            this.log = log;
            
            sendButton.OnClick += SendLog;
            exitButton.OnClick += Close;
        }

        private async void SendLog()
        {
            try
            {
                logData = new LogData()
                {
                    phone = SystemInfo.deviceModel,
                    build_version = $"v{Application.version}",
                    ram = SystemInfo.systemMemorySize.ToString(),
                    error = log,
                    description = inputField.text,
                };
            
                await restManager.PostAsync<Response<string>>(googleScriptUrl,
                    logData, new Header("Content-Type", "application/json"));

                Close();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void Close()
        {
            gameObject.SetActive(false);
            
            sendButton.OnClick -= SendLog;
            exitButton.OnClick -= Close;
        }
    }
}