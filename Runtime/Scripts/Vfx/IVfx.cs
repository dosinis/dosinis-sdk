using UnityEngine;

namespace DosinisSDK.Vfx
{
    public interface IVfx
    {
        void Play();
        void Stop(bool withChildren = true);
        bool IsAlive();
        bool Expired();
        bool IsPlaying { get; }
        Transform Transform { get; }
        GameObject GameObject { get; }
    }
}
