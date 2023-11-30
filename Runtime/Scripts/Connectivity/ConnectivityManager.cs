using System.Collections;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace DosinisSDK.Connectivity
{
    public class ConnectivityManager : Module, IConnectivityManager
    {
        private Observable<bool> connected;
        private ICoroutineManager coroutineManager;
        
        public IObservable<bool> Connected => connected;

        private const float CHECK_FREQUENCY = 30f;
        
        protected override void OnInit(IApp app)
        {
            coroutineManager = app.Coroutine;
            app.Timer.Repeat(CHECK_FREQUENCY, int.MaxValue, (_) =>
            {
                RefreshConnectionStatus();
            });
        }

        public void RefreshConnectionStatus()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                connected.Value = false;
                return;
            }

            coroutineManager.Begin(CheckConnection());
        }
        
        private IEnumerator CheckConnection()
        {
            var www = new UnityWebRequest("https://www.google.com");
            
            yield return www;
            
            connected.Value = www.error == null;
        }
    }
}