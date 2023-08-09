using System;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Particles
{
    public interface IParticlesManager : IModule
    {
        void PlayAtPoint(ParticleSystem vfx, Vector3 point, AudioClip sfx = null, Action done = null);
        
        long Play(ParticleSystem vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null, Vector3 offset = default,
            Action done = null);

        void Stop(ParticleSystem vfx, long hash);
        
        void StopAll(ParticleSystem vfx);
    }
}
