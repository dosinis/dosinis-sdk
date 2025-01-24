using DosinisSDK.Core;
using DosinisSDK.Pool;
using UnityEngine;
using UnityEngine.UI;
using Button = DosinisSDK.Core.Button;

namespace DosinisSDK.LogConsole
{
    public class LogConsole : BehaviourModule, IProcessable
    {
        [SerializeField] private GameObjectPool logEntryPool;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button pauseButton;

        private VerticalLayoutGroup verticalLayoutGroup;
        private ITimer timer;
        private bool paused;
        
        protected override void OnInit(IApp app)
        {
            timer = app.Timer;
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>(true);
            
            Application.logMessageReceived += HandleLog;

            closeButton.OnClick += () =>
            {
                gameObject.SetActive(false);
            };

            clearButton.OnClick += () =>
            {
                logEntryPool.ReturnAll();
            };

            pauseButton.OnClick += () =>
            {
                paused = !paused;
            };

            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            
            verticalLayoutGroup.enabled = false;
            
            timer.SkipFrame(() =>
            {
                verticalLayoutGroup.enabled = true;
            });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        protected override void OnDispose()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string condition, string stacktrace, LogType type)
        {
            if (paused)
                return;
            
            logEntryPool.Take<LogEntry>().Setup(condition, stacktrace, type);
        }

        public void Process(in float delta)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (gameObject.activeSelf == false)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}