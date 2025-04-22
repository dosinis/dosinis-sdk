using System;
using DosinisSDK.Core;
using DosinisSDK.Pool;
using DosinisSDK.Rest;
using UnityEngine;
using UnityEngine.UI;
using Button = DosinisSDK.Core.Button;

namespace DosinisSDK
{
    public class LogConsole : MonoBehaviour, IDisposable
    {
        [SerializeField] private SendLogWidget sendLogWidget;
        [SerializeField] private GameObjectPool logEntryPool;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button interactWindowButton;
        [SerializeField] private GameObject logsContent;

        private VerticalLayoutGroup verticalLayoutGroup;
        private ITimer timer;
        private bool paused;

        public void OnInit(IApp app, string googleScriptUrl)
        {
            var restManager = app.GetModule<RestManager>();
            
            timer = app.Timer;
            
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>(true);

            Application.logMessageReceived += HandleLog;

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

            sendLogWidget.Init(restManager, googleScriptUrl);
            
            Close();
        }
        
        public void Dispose()
        {
            Application.logMessageReceived -= HandleLog;
        }
        
        public void Process(in float delta)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
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
    }
}