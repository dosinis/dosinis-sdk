using DosinisSDK.Core;
using TMPro;
using UnityEngine;

namespace DosinisSDK
{
    public class LogEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text conditionText;
        [SerializeField] private TMP_Text stacktraceText;
        [SerializeField] private Button sendLogButton;
        
        private SendLogWidget sendLogWidget;

        public void Setup(string condition, string stacktrace,
            LogType type, SendLogWidget sendLogWidget)
        {
            this.sendLogWidget = sendLogWidget;
            this.conditionText.text = condition;
            this.stacktraceText.text = stacktrace;
            
            ApplyTextsColor(type);

            sendLogButton.OnClick += SendLog;
        }

        private void OnDestroy()
        {
            sendLogButton.OnClick -= SendLog;
        }
        
        private void ApplyTextsColor(LogType type)
        {
            Color textColor = Color.white;
            
            if (type == LogType.Error || type == LogType.Exception)
            {
                textColor = Color.red;
            }
            else if (type == LogType.Warning)
            {
                textColor = Color.yellow;
            }

            conditionText.color = textColor;
            stacktraceText.color = textColor;
        }
        
        private void SendLog()
        {
            sendLogWidget.Show($"{conditionText.text}:\n{stacktraceText.text}");
        }
    }
}
