using System;
using System.Collections;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class VfxManager : BehaviourModule, IVfxManager, IProcessable
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
            
            ps.Transform.forward = forward;
            
            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
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

            var h = Helper.GetRandomLong();
            effectsCache.Add(h, ps);

            if (forceKeepOrientation)
            {
                forcedOrientationVfxCache.Add(h, ps);
            }

            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                coroutineManager.Begin(WaitForEffect(ps, done));
            }

            return h;
        }

        public void Stop(IVfx vfx, long hash)
        {
            if (effectPools.ContainsKey(vfx) == false)
            {
                return;
            }

            if (effectsCache.ContainsKey(hash) == false)
            {
                return;
            }

            effectsCache[hash].Stop();
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

        public void Process(in float delta)
        {
            itemsToRemove.Clear();
            
            foreach (var effect in effectsCache)
            {
                if (effect.Value == null)
                {
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
