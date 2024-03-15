using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using Object = UnityEngine.Object;

namespace DosinisSDK.Assets
{
    [Serializable]
    public class AssetLink
    {
#if UNITY_EDITOR
        /// <summary>
        /// This constructor is EDITOR ONLY
        /// </summary>
        /// <param name="obj"></param>
        public AssetLink(Object obj)
        {
            path = EditorUtils.GetAssetPath(obj);
        }
#endif
            
        public AssetLink(string path)
        {
            this.path = path;
        }
        
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
