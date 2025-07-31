using UnityEngine;

namespace DosinisSDK.Vfx
{
    public interface IVfx
    {
        string Id { get; }
        void Play();
        void Stop(bool withChildren = true, bool clear = true);
        bool IsAlive();
        bool Expired();
        bool IsPlaying { get; }
        Transform Transform { get; }
        GameObject GameObject { get; }
    }
}
