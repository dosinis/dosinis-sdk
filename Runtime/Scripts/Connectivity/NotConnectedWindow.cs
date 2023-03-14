using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Connectivity
{
    public class NotConnectedWindow : Window
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private bool pauseOnShown = true;
        
        private IConnectivityManager connectionManager;
        
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
                Time.timeScale = float.Epsilon;
            }
        }
        
        protected override void BeforeHidden()
        {
            if (pauseOnShown)
            {
                Time.timeScale = 1f;
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