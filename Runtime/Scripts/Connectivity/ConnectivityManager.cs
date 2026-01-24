using System.Collections;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace DosinisSDK.Connectivity
{
    public class ConnectivityManager : Module, IConnectivityManager
    {
        private readonly Observable<bool> connected = new(true);
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
            coroutineManager.Begin(RefreshConnectionStatusRoutine());
        }

        public IEnumerator RefreshConnectionStatusRoutine()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                connected.Value = false;

                yield break;
            }

            var checkConnection = CheckConnection();
            
            coroutineManager.Begin(checkConnection);

            yield return checkConnection;
        }
        
        private IEnumerator CheckConnection()
        {
            var www = new UnityWebRequest("https://www.google.com");
            
            yield return www;
            
            connected.Value = www.error == null;
        }
    }
}