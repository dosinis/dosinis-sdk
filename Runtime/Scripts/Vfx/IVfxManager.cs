using System;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public interface IVfxManager : IModule
    {
        void PlayAtPoint(IVfx vfx, Vector3 point, Vector3 forward, AudioClip sfx = null, Action done = null);
        
        /// <summary>
        /// Returns VFX hash for further control
        /// </summary>
        /// <param name="vfx">Source VFX used for pooling</param>
        /// <param name="forceKeepOrientation">Force local rotation to identity?</param>
        /// <param name="parent">Override parent</param>
        /// <param name="sfx">Sound effect</param>
        /// <param name="offset">Offset from parent</param>
        /// <param name="done">Callback when vfx is no longer alive</param>
        /// <returns></returns>
        long Play(IVfx vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null, Vector3 offset = default,
            Action done = null);

        /// <summary>
        /// Stops the provided VFX
        /// </summary>
        /// <param name="vfx">Source VFX, that was used for pooling</param>
        /// <param name="hash">Hash generated by Play method</param>
        void Stop(IVfx vfx, long hash);
        
        void StopAll(IVfx vfx);
    }
}