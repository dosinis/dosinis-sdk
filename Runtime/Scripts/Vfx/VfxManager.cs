using System;
using System.Collections;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class VfxManager : BehaviourModule, IVfxManager, ILateProcessable
    {
        private readonly Dictionary<IVfx, VfxPool> effectPools = new();

        private readonly Dictionary<long, IVfx> forcedOrientationVfxCache = new();
        private readonly Dictionary<long, IVfx> effectsCache = new();

        private readonly List<long> itemsToRemove = new();
        private ICoroutineManager coroutineManager;

        protected override void OnInit(IApp app)
        {
            coroutineManager = app.Coroutine;
        }

        public void PlayAtPoint(IVfx vfx, Vector3 point, Vector3 forward, AudioClip sfx = null, Action done = null)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                effectPools.Add(vfx, VfxPool.Create(vfx));
            }

            var ps = effectPools[vfx].Play(point);

            if (forward != Vector3.zero)
            {
                ps.Transform.forward = forward;
            }
            
            if (sfx != null)
            {
                sfx.PlayOneShotSafeAtPoint(point);
            }
            
            if (done != null)
            {
                coroutineManager.Begin(WaitForEffect(ps, done));
            }
        }

        public long Play(IVfx vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null, Vector3 offset = default, Action done = null)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                effectPools.Add(vfx, VfxPool.Create(vfx));
            }

            IVfx ps;
            
            if (parent != null)
            {
                ps = effectPools[vfx].Play(parent);
            }
            else
            {
                ps = effectPools[vfx].Play();
            }

            ps.Transform.localPosition += offset;

            var key = Helper.GetRandomLong();
            effectsCache.Add(key, ps);

            if (forceKeepOrientation)
            {
                forcedOrientationVfxCache.Add(key, ps);
            }

            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                coroutineManager.Begin(WaitForEffect(ps, done));
            }

            return key;
        }

        public bool IsPlaying(IVfx vfx, long key)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                return false;
            }

            if (effectsCache.ContainsKey(key) == false)
            {
                return false;
            }

            return effectsCache[key].IsAlive();
        }

        public void Stop(IVfx vfx, long key, bool clearParticles = true)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                Debug.LogWarning($"Vfx pool for {vfx.GameObject.name} not found!");
                return;
            }

            if (effectsCache.ContainsKey(key) == false)
            {
                return;
            }
            
            effectsCache[key].Stop(clear: clearParticles);
        }
        
        public void StopAll(IVfx vfx)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                return;
            }

            effectPools[vfx].StopAll();
        }

        private IEnumerator WaitForEffect(IVfx vfx, Action done)
        {
            yield return new WaitUntil(vfx.IsAlive);
            
            done?.Invoke();
        }

        void ILateProcessable.LateProcess(in float delta)
        {
            itemsToRemove.Clear();
            
            foreach (var effect in effectsCache)
            {
                if (effect.Value == null)
                {
                    itemsToRemove.Add(effect.Key);
                    continue;
                }
                
                if (effect.Value.IsAlive() == false)
                {
                    itemsToRemove.Add(effect.Key);
                    continue;
                }

                if (forcedOrientationVfxCache.ContainsKey(effect.Key))
                {
                    effect.Value.Transform.rotation = Quaternion.identity;
                }
            }
            
            foreach (var item in itemsToRemove)
            {
                effectsCache.Remove(item);
                
                if (forcedOrientationVfxCache.ContainsKey(item))
                {
                    forcedOrientationVfxCache.Remove(item);
                }
            }
        }
    }
}
