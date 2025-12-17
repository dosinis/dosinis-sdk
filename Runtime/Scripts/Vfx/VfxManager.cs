using System;
using System.Collections;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DosinisSDK.Vfx
{
    public class VfxManager : BehaviourModule, IVfxManager, ILateProcessable
    {
        private readonly Dictionary<string, VfxPool> effectPools = new();

        private readonly Dictionary<long, IVfx> forcedOrientationVfxCache = new();
        private readonly Dictionary<long, IVfx> effectsCache = new();

        private readonly List<long> itemsToRemove = new();
        private ICoroutineManager coroutineManager;

        protected override void OnInit(IApp app)
        {
            coroutineManager = app.Coroutine;
        }

        public long PlayAtPoint(IVfx vfx, Vector3 point, Vector3 forward, AudioClip sfx = null, Action done = null)
        {
            if (vfx == null)
            {
                LogError("Vfx is null, cannot play effect at point.");
                return -1;
            }
            
            ValidatePool(vfx);
            
            if (effectPools.ContainsKey(vfx.Id) == false)
            {
                effectPools.Add(vfx.Id, VfxPool.Create(vfx));
            }

            var ps = effectPools[vfx.Id].Play(point);

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
            
            var key = Helper.GetRandomLong();
            effectsCache.Add(key, ps);
            
            return key;
        }

        public long Play(IVfx vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null,
            Vector3 offset = default, Action done = null, bool forceKeepLocalOrientation = false)
        {
            if (vfx == null)
            {
                LogError("Vfx is null, cannot play effect.");
                return -1;
            }

            ValidatePool(vfx);
            
            if (effectPools.ContainsKey(vfx.Id) == false)
            {
                effectPools.Add(vfx.Id, VfxPool.Create(vfx));
            }

            IVfx ps;
            
            if (parent != null)
            {
                ps = effectPools[vfx.Id].Play(parent);
            }
            else
            {
                ps = effectPools[vfx.Id].Play();
            }

            ps.Transform.localPosition += offset;

            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                coroutineManager.Begin(WaitForEffect(ps, done));
            }
            
            var key = Helper.GetRandomLong();
            effectsCache.Add(key, ps);

            if (forceKeepLocalOrientation)
            {
                ps.Transform.localRotation = Quaternion.identity;
            }

            if (forceKeepOrientation)
            {
                forcedOrientationVfxCache.Add(key, ps);
            }

            return key;
        }

        public bool IsPlaying(IVfx vfx, long key)
        {
            if (effectPools.ContainsKey(vfx.Id) == false)
            {
                return false;
            }

            if (effectsCache.ContainsKey(key) == false)
            {
                return false;
            }

            return effectsCache[key].IsAlive();
        }

        public void Stop(long key, bool clearParticles = true)
        {
            if (effectsCache.ContainsKey(key) == false)
            {
                return;
            }

            if (effectsCache[key].Exists() == false)
            {
                return;
            }
            
            effectsCache[key].Stop(clear: clearParticles);
        }
        
        public void StopAll(IVfx vfx)
        {
            if (effectPools.ContainsKey(vfx.Id) == false)
            {
                return;
            }

            effectPools[vfx.Id].StopAll();
        }

        public void ValidatePool(IVfx vfx)
        {
            if (effectPools.TryGetValue(vfx.Id, out var pool) && pool.IsValid() == false)
            {
                DisposePool(vfx);
            }
        }

        public void DisposePool(IVfx vfx)
        {
            if (effectPools.ContainsKey(vfx.Id) == false)
            {
                return;
            }
            
            effectPools[vfx.Id].Dispose();
            effectPools.Remove(vfx.Id);
            
            foreach (var item in effectsCache)
            {
                if (item.Value == vfx)
                {
                    effectsCache.Remove(item.Key);
                }
            }
            
            foreach (var item in forcedOrientationVfxCache)
            {
                if (item.Value == vfx)
                {
                    forcedOrientationVfxCache.Remove(item.Key);
                }
            }
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
                if (effect.Value.Exists() == false)
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
    
    public static class VfxExtensions
    {
        public static bool Exists(this IVfx v)
        {
            if(v == null) 
                return false;
            
            if(v is Object o)
                return o != null;
            
            return true;
        }
    }
}
