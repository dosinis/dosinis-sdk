using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Assets
{
    public class AssetProviderResources : IAssetProvider
    {
        public async Task<T> GetAssetAsync<T>(string path) where T : Object
        {
            var op = Resources.LoadAsync<T>(path);
            
            bool isDone = false;
            
            op.completed += (_) =>
            {
                isDone = true;
            };

            while (isDone == false)
            {
                await Task.Yield();
            }
            
            return op.asset as T;
        }

        public void GetAssetAsync<T>(string path, Action<T> callback) where T : Object
        {
            var op = Resources.LoadAsync<T>(path);
            
            op.completed += (_) =>
            {
                callback?.Invoke(op.asset as T);
            };
        }

        public T GetAsset<T>(string path) where T : Object
        {
            var item = Resources.Load(path) as T;
            
            if (item == null)
            {
                Debug.LogError("Item not found: " + path);
            }

            return item;
        }
    }
}
