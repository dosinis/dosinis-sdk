using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Rest
{
    public interface IRestManager : IModule
    {
        void GetTexture(string url, Action<Response<Texture>> callback);
        Task<Response<Texture>> GetTextureAsync(string url);
        void Get(string url, Action<Response> callback);
        void Get<T>(string url, Action<Response<T>> callback);
        void Post<T>(string url, object parameter, Action<Response<T>> callback, params Header[] headers);
        void Post<T>(string url, Action<Response<T>> callback, params Header[] headers);
        Task<Response<T>> GetAsync<T>(string url);
        Task<Response> GetAsync(string url);
        Task<Response<T>> PostAsync<T>(string url, object parameter, params Header[] headers);
        Task<Response<T>> PostAsync<T>(string url, params Header[] headers);
    }
}
