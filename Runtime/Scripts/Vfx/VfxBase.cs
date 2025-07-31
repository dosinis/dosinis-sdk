using DosinisSDK.Inspector;
using DosinisSDK.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DosinisSDK.Vfx
{
    public abstract class VfxBase : MonoBehaviour, IVfx
    {
        [SerializeField] protected string id;

        public virtual string Id => string.IsNullOrEmpty(id) ? gameObject.name : id;

        public abstract void Play();
        public abstract void Stop(bool withChildren = true, bool clear = true);
        public abstract bool IsAlive();
        public abstract bool Expired();
        public abstract bool IsPlaying { get; }
        public abstract Transform Transform { get; }
        public abstract GameObject GameObject { get; }

#if UNITY_EDITOR
        [Button]
        protected void SetUniqueId()
        {
            id = Helper.ShortUid();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}