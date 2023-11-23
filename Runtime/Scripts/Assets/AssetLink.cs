using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using Object = UnityEngine.Object;

namespace DosinisSDK.Assets
{
    [Serializable]
    public class AssetLink
    {
        public string path;
        
        public T GetAsset<T>() where T : Object
        {
            return App.Core.GetModule<GlobalAssetsManager>().GetAsset<T>(path);
        }
        
        public void GetAssetAsync<T>(Action<T> callback) where T : Object
        {
            App.Core.GetModule<GlobalAssetsManager>().GetAssetAsync(path, callback);
        }
        
        public async Task<T> GetAssetAsync<T>() where T : Object
        {
            return await App.Core.GetModule<GlobalAssetsManager>().GetAssetAsync<T>(path);
        }
    }
}
