using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Connectivity
{
    public class NotConnectedWindow : Window
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private bool pauseOnShown = true;
        
        private IConnectivityManager connectionManager;
        private const string PAUSE_ID = "not-connected-window";
        
        protected override void OnInit(IApp app)
        {
            connectionManager = app.GetModule<IConnectivityManager>();
            connectionManager.Connected.OnValueChanged += OnConnectedValueChanged;
            
            app.Timer.Delay(1f, () =>
            {
                if (connectionManager.Connected.Value == false)
                {
                    Show();
                }
            });

            retryButton.OnClick += connectionManager.RefreshConnectionStatus;
        }

        protected override void BeforeShown()
        {
            if (pauseOnShown)
            {
                TimeScaleUtils.Pause(PAUSE_ID);
            }
        }
        
        protected override void BeforeHidden()
        {
            if (pauseOnShown)
            {
                TimeScaleUtils.Resume(PAUSE_ID);
            }
        }

        private void OnConnectedValueChanged(bool connected)
        {
            if (connected == false)
            {
                if (IsShown == false)
                {
                    Show();
                }
            }
            else
            {
                if (IsShown)
                {
                    Hide();
                }
            }
        }
    }
}