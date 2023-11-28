using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using DosinisSDK.Core;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace DosinisSDK.Rest
{
    public class RestManager : Module, IRestManager
    {
        private ICoroutineManager coroutineManager;
        
        protected override void OnInit(IApp app)
        {
            coroutineManager = app.Coroutine;
        }
        
        // Requests
        
        public void Get<T>(string url, Action<T> callback)
        {
            coroutineManager.Begin(GetInternal(url, callback));
        }
        
        public void Post<T>(string url, object data, Action<T> callback)
        {
            coroutineManager.Begin(PostInternal(url, data, callback));
        }
        
        public async Task<T> GetAsync<T>(string url)
        {
            var request = UnityWebRequest.Get(url);
            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }
            
            LogOperation(request);
            
            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }
        
        public async Task<T> PostAsync<T>(string url, object data)
        {
            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }
            
            LogOperation(request);
            
            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }
        
        // Internal
        
        private IEnumerator GetInternal<T>(string url, Action<T> callback)
        {
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            
            LogOperation(request);
            
            var result = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            callback?.Invoke(result);
        }
        
        private IEnumerator PostInternal<T>(string url, object data, Action<T> callback)
        {
            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            LogOperation(request);

            var result = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            callback?.Invoke(result);
        }

        private void LogOperation(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                LogError($"Request {request.result}. {request.responseCode}: {request.error}");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Log($"Success. {request.responseCode}: {request.downloadHandler.text}");
            }
        }
    }
}
