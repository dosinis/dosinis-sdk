using System.Threading.Tasks;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Assets
{
    public class GlobalAssetsManager : Module
    {
        private IAssetProvider provider;
        
        protected override void OnInit(IApp app)
        {
            
        }

        public void SetProvider(IAssetProvider provider)
        {
            this.provider = provider;
        }
        
        public T GetAsset<T>(string path) where T : Object
        {
            return provider.GetAsset<T>(path);
        }
        
        public void GetAssetAsync<T>(string path, System.Action<T> callback) where T : Object
        {
            provider.GetAssetAsync(path, callback);
        }
        
        public async Task<T> GetAssetAsync<T>(string path) where T : Object
        {
            return await provider.GetAssetAsync<T>(path);
        }
    }
}
