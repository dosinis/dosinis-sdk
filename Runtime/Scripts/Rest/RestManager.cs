using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using DosinisSDK.Core;
using Newtonsoft.Json;
using UnityEngine;
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

        #region GET

        public void GetTexture(string url, Action<Response<Texture>> callback)
        {
            coroutineManager.Begin(GetTextureInternal(url, callback));
        }

        public async Task<Response<Texture>> GetTextureAsync(string url)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            
            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;
            
            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }

            LogOperation(request);

            if (request.result != UnityWebRequest.Result.Success)
            {
                return new Response<Texture>(null, request.responseCode, (Result)request.result, request.error);
            }
            
            return new Response<Texture>(((DownloadHandlerTexture)request.downloadHandler).texture, request.responseCode, (Result)request.result, request.error);
        }

        public void Get(string url, Action<Response> callback)
        {
            coroutineManager.Begin(GetInternal(url, callback));
        }

        public void Get<T>(string url, Action<Response<T>> callback)
        {
            coroutineManager.Begin(GetInternal(url, callback));
        }

        public async Task<Response<T>> GetAsync<T>(string url)
        {
            using var request = CreateGetRequest(url);

            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }

            LogOperation(request);

            return CreateResponse<T>(request);
        }

        public async Task<Response> GetAsync(string url)
        {
            using var request = CreateGetRequest(url);

            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }

            LogOperation(request);

            return new Response(request.downloadHandler.text, request.responseCode, (Result)request.result, request.error);
        }

        #endregion

        #region POST

        public void Post<T>(string url, object parameter, Action<Response<T>> callback, params Header[] headers)
        {
            coroutineManager.Begin(PostInternal(url, parameter, callback, headers));
        }

        public void Post<T>(string url, Action<Response<T>> callback, params Header[] headers)
        {
            coroutineManager.Begin(PostInternal(url, null, callback, headers));
        }

        public async Task<Response<T>> PostAsync<T>(string url, object parameter, params Header[] headers)
        {
            using var request = CreatePostRequest(url, parameter, headers);
            request.SendWebRequest();

            while (request.downloadHandler.isDone == false)
            {
                await Task.Yield();
            }

            LogOperation(request);

            return CreateResponse<T>(request);
        }

        public async Task<Response<T>> PostAsync<T>(string url, params Header[] headers)
        {
            return await PostAsync<T>(url, null, headers);
        }

        #endregion
        
        // Internal

        #region INTERNAL
        private Response<T> CreateResponse<T>(UnityWebRequest request)
        {
            T resultObject = default;

            try
            {
                string responseText = request.downloadHandler.text;

                if (IsSimpleText(responseText))
                {
                    responseText = $"{{ \"response\": \"{responseText}\" }}";
                }
                
                resultObject = JsonConvert.DeserializeObject<T>(responseText);
            }
            catch (Exception ex)
            {
                LogError(ex.Message + "\n" + ex.StackTrace);
            }

            var response = new Response<T>(resultObject, request.responseCode, (Result)request.result, request.error);

            return response;
        }
        
        private IEnumerator GetInternal<T>(string url, Action<Response<T>> callback)
        {
            using var request = CreateGetRequest(url);
            yield return request.SendWebRequest();

            LogOperation(request);

            callback?.Invoke(CreateResponse<T>(request));
        }

        private IEnumerator GetInternal(string url, Action<Response> callback)
        {
            using var request = CreateGetRequest(url);
            yield return request.SendWebRequest();

            LogOperation(request);

            callback?.Invoke(new Response(request.downloadHandler.text, request.responseCode, (Result)request.result,
                request.error));
        }

        private IEnumerator GetTextureInternal(string url, Action<Response<Texture>> callback)
        {
            var request = UnityWebRequestTexture.GetTexture(url);

            yield return request.SendWebRequest();

            LogOperation(request);

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(new Response<Texture>(null, request.responseCode, (Result)request.result, request.error));
                yield break;
            }

            Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            callback?.Invoke(new Response<Texture>(myTexture, request.responseCode, (Result)request.result, request.error));
        }

        private IEnumerator PostInternal<T>(string url, object parameter, Action<Response<T>> callback, params Header[] headers)
        {
            using var request = CreatePostRequest(url, parameter, headers);
            yield return request.SendWebRequest();

            LogOperation(request);

            callback?.Invoke(CreateResponse<T>(request));
        }

        private UnityWebRequest CreatePostRequest(string url, object parameter, params Header[] headers)
        {
            var request = new UnityWebRequest(url, "POST");

            if (parameter != null)
            {
                request.uploadHandler =
                    new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameter)));
                request.SetRequestHeader("Content-Type", "application/json");
            }
            
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.key, header.value);
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;

            return request;
        }

        private UnityWebRequest CreateGetRequest(string url)
        {
            var request = UnityWebRequest.Get(url);
            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;

            return request;
        }

        private void LogOperation(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                LogError($"Request {request.result}. {request.responseCode}: {request.error}. Text: {request.downloadHandler.text}");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Log($"Success. {request.responseCode}: {request.downloadHandler.text}");
            }
        }
        
        private bool IsSimpleText(string responseText)
        {
            if (string.IsNullOrWhiteSpace(responseText))
                return true;

            var trimmed = responseText.Trim();

            return !(trimmed.StartsWith("{") && trimmed.EndsWith("}"));
        }
        #endregion
    }
}