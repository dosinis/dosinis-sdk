using TMPro;
using UnityEngine;

namespace DosinisSDK
{
    public class LogEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text conditionText;
        [SerializeField] private TMP_Text stacktraceText;

        public void Setup(string condition, string stacktrace, LogType type)
        {
            conditionText.text = condition;
            stacktraceText.text = stacktrace;
            
            if (type == LogType.Error || type == LogType.Exception)
            {
                conditionText.color = Color.red;
            }
            else if (type == LogType.Warning)
            {
                conditionText.color = Color.yellow;
            }
            else
            {
                conditionText.color = Color.white;
            }
        }
    }
}
