using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Core
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

            return ConvertAsset<T>(op.asset, path);
        }

        public void GetAssetAsync<T>(string path, Action<T> callback) where T : Object
        {
            var op = Resources.LoadAsync<T>(path);
            
            op.completed += (_) =>
            {
                callback?.Invoke(ConvertAsset<T>(op.asset, path));
            };
        }

        public T GetAsset<T>(string path) where T : Object
        {
            var asset = Resources.Load(path);

            return ConvertAsset<T>(asset, path);
        }

        private T ConvertAsset<T>(Object asset, string path) where T : Object
        {
            if (asset == null)
            {
                Debug.LogError("Item not found: " + path);
                return null;
            }

            var item = asset as T;

            if (item == null)
            {
                var gameObject = asset as GameObject;

                if (gameObject != null)
                {
                    item = gameObject.GetComponent<T>();
                }
            }

            if (item == null)
            {
                Debug.LogError("Asset found but not of type " + typeof(T).Name + ": " + path);
            }

            return item;
        }
    }
}
