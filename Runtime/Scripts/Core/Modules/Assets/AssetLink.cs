using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Core
{
    [Serializable]
    public class AssetLink<T> : AssetLink where T : Object
    {
        public Type Type => typeof(T);
        
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
            path = GetAssetPathResourcesAdjusted(obj);
            guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        }
        
        private string GetAssetPathResourcesAdjusted(Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            
            if (assetPath.StartsWith("Assets/Resources/"))
            {
                assetPath = assetPath.Replace("Assets/Resources/", "");
                assetPath = assetPath.RemovePathExtension();
            }

            return assetPath;
        }

        public string Guid => guid;
#endif
        public string Path => path;
        
        public bool IsValid => string.IsNullOrEmpty(path) == false;
        
        public T GetAsset<T>() where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
            }
#endif
            return App.Core.GetModule<GlobalAssetsManager>().GetAsset<T>(path);
        }
        
        public void GetAssetAsync<T>(Action<T> callback) where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
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
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
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
