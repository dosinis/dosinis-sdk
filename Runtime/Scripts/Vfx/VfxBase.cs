using UnityEngine;

namespace DosinisSDK.Vfx
{
    public abstract class VfxBase : MonoBehaviour, IVfx
    {
        public abstract void Play();
        public abstract void Stop(bool withChildren = true);
        public abstract bool IsAlive();
        public abstract bool Expired();
        public abstract bool IsPlaying { get; }
        public abstract Transform Transform { get; }
        public abstract GameObject GameObject { get; }
    }
}
