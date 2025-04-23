using System;
using DosinisSDK.Core;
using DosinisSDK.Pool;
using DosinisSDK.Rest;
using UnityEngine;
using UnityEngine.UI;
using Button = DosinisSDK.Core.Button;

namespace DosinisSDK.LogConsole
{
    public class LogConsole : BehaviourModule, IProcessable
    {
        [SerializeField] private SendLogWidget sendLogWidget;
        [SerializeField] private GameObjectPool logEntryPool;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button interactWindowButton;
        [SerializeField] private GameObject logsContent;
        [SerializeField] private ScrollRect scrollRect;

        private VerticalLayoutGroup verticalLayoutGroup;
        private ITimer timer;
        private bool paused;
        private LogConfig logConfig;

        protected override void OnInit(IApp app)
        {
            timer = app.Timer;
            logConfig = GetConfigAs<LogConfig>();
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>(true);

            var restManager = app.GetModule<RestManager>();
            var logConsoleApi = new LogConsoleApi(restManager, logConfig.GetGoogleScriptUrl);

            sendLogWidget.Init(logConsoleApi);

            Close();
            SubscribeButtons();
            DontDestroyOnLoad(gameObject);
            
            Application.logMessageReceived += HandleLog;
        }
        
        protected override void OnDispose()
        {
            Application.logMessageReceived -= HandleLog;
        }
        
        public void Process(in float delta)
        {
            if (logConfig.IsOpenByKey && Input.GetKeyDown(KeyCode.BackQuote))
            {
                InteractWithWindow();
            }
        }
        
        private void Open()
        {
            logsContent.SetActive(true);
            
            verticalLayoutGroup.enabled = false;
            
            timer.SkipFrame(() =>
            {
                verticalLayoutGroup.enabled = true;
                
                timer.SkipFrame(() =>
                {
                    scrollRect.verticalNormalizedPosition = 0f;
                });
            });
        }

        private void Close()
        {
            logsContent.SetActive(false);
        }
        
        private void HandleLog(string condition, string stacktrace, LogType type)
        {
            if (paused)
                return;
            
            logEntryPool.Take<LogEntry>().Setup(condition, stacktrace, type, sendLogWidget);
        }
        
        private void InteractWithWindow()
        {
            if (logsContent.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
        
        private void SubscribeButtons()
        {
            clearButton.OnClick += () =>
            {
                logEntryPool.ReturnAll();
            };

            pauseButton.OnClick += () =>
            {
                paused = !paused;
            };

            closeButton.OnClick += Close;
            interactWindowButton.OnClick += InteractWithWindow;
        }
    }
}