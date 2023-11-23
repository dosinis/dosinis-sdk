using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace DosinisSDK.Assets
{
    public interface IAssetProvider
    {
        Task<T> GetAssetAsync<T>(string path) where T : Object;
        void GetAssetAsync<T>(string path, Action<T> callback) where T : Object;
        T GetAsset<T>(string path) where T : Object;
    }
}
