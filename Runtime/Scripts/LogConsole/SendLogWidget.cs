using System;
using DosinisSDK.Core;
using DosinisSDK.Rest;
using TMPro;
using UnityEngine;

namespace DosinisSDK.LogConsole
{
    public class SendLogWidget : MonoBehaviour
    {
        [SerializeField] private TMP_InputField descriptionInputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button exitButton;
        
        private IRestManager restManager;
        private LogConsoleApi logConsoleApi;
        private string log;

        public void Init(LogConsoleApi logConsoleApi)
        {
            this.logConsoleApi = logConsoleApi;
            
            gameObject.SetActive(false);
        }
        
        public void Show(string log)
        {
            gameObject.SetActive(true);
            
            this.log = log;
            
            sendButton.OnClick += SendLog;
            exitButton.OnClick += Close;
        }

        private void SendLog()
        {
            logConsoleApi.PostDataAsync(log, descriptionInputField.text);
                
            Close();
        }

        private void Close()
        {
            gameObject.SetActive(false);
            
            sendButton.OnClick -= SendLog;
            exitButton.OnClick -= Close;
        }
    }
}