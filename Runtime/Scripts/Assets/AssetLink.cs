using System;
using System.Threading.Tasks;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Assets
{
    [Serializable]
    public class AssetLink<T> : AssetLink where T : Object
    {
#if UNITY_EDITOR
        public AssetLink(Object obj) : base(obj)
        {
        }
#endif
        
        public T GetAsset()
        {
            return base.GetAsset<T>();
        }
        
        public void GetAssetAsync(Action<T> callback)
        {
            base.GetAssetAsync(callback);
        }
        
        public async Task<T> GetAssetAsync()
        {
            return await base.GetAssetAsync<T>();
        }
    }
    
    [Serializable]
    public class AssetLink
    {
        [SerializeField] private string path;
        
#if UNITY_EDITOR
        [SerializeField] private string guid;
        
        /// <summary>
        /// This constructor is EDITOR ONLY
        /// </summary>
        /// <param name="obj"></param>
        public AssetLink(Object obj)
        {
            path = EditorUtils.GetAssetPathResourcesAdjusted(obj);
            guid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(obj));
        }
        
        public string Guid => guid;
#endif
        public string Path => path;
        
        public T GetAsset<T>() where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
#endif
            return App.Core.GetModule<GlobalAssetsManager>().GetAsset<T>(path);
        }
        
        public void GetAssetAsync<T>(Action<T> callback) where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                callback?.Invoke(asset);
                return;
            }
#endif
            App.Core.GetModule<GlobalAssetsManager>().GetAssetAsync(path, callback);
        }
        
        public async Task<T> GetAssetAsync<T>() where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
#endif
            return await App.Core.GetModule<GlobalAssetsManager>().GetAssetAsync<T>(path);
        }

        public override bool Equals(object obj)
        {
            if (obj is AssetLink other)
            {
                return path == other.path;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 17 * 31 + Path.GetHashCode();
        }
    }
}
