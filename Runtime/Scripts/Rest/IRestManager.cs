using System;
using System.Threading.Tasks;
using DosinisSDK.Core;

namespace DosinisSDK.Rest
{
    public interface IRestManager : IModule
    {
        void Get<T>(string url, Action<T> callback);
        void Post<T>(string url, object data, Action<T> callback);
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, object data);
    }
}
