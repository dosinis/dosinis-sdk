using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using DosinisSDK.Pool;
using DosinisSDK.Rest;
using UnityEngine;
using UnityEngine.UI;
using Button = DosinisSDK.Core.Button;

namespace DosinisSDK.LogConsole
{
    public class LogConsole : BehaviourModule, IProcessable, ILogConsole
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
        private RestManager restManager;

        protected override void OnInit(IApp app)
        {
            timer = app.Timer;
            logConfig = GetConfigAs<LogConfig>();
            restManager = app.GetModule<RestManager>();
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>(true);

            sendLogWidget.Init(this);

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

        public async Task PostDataAsync(string log, string description)
        {
            LogData logData = new LogData()
            {
                phone = SystemInfo.deviceModel,
                buildVersion = $"v{Application.version}",
                ram = SystemInfo.systemMemorySize.ToString(),
                error = log,
                description = description,
            };

            await restManager.PostAsync<Response<object>>(logConfig.GetGoogleScriptUrl,
                logData, new Header("Content-Type", "application/json"));
        }

        private void Open()
        {
            logsContent.SetActive(true);

            verticalLayoutGroup.enabled = false;

            timer.SkipFrame(() =>
            {
                verticalLayoutGroup.enabled = true;

                timer.SkipFrame(() => { scrollRect.verticalNormalizedPosition = 0f; });
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
            clearButton.OnClick += () => { logEntryPool.ReturnAll(); };

            pauseButton.OnClick += () => { paused = !paused; };

            closeButton.OnClick += Close;
            interactWindowButton.OnClick += InteractWithWindow;
        }
    }
}