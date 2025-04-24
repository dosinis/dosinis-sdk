using DosinisSDK.Core;
using TMPro;
using UnityEngine;

namespace DosinisSDK.LogConsole
{
    public class SendLogWidget : MonoBehaviour
    {
        [SerializeField] private TMP_InputField descriptionInputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button exitButton;

        private ILogConsole logConsole;
        private string log;

        public void Init(ILogConsole logConsole)
        {
            this.logConsole = logConsole;
            
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
            logConsole.PostDataAsync(log, descriptionInputField.text);
                
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