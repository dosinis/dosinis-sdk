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
        private readonly Dictionary<IVfx, VfxPool> effects = new();

        private readonly Dictionary<long, IVfx> forcedOrientationVfxCache = new();
        private readonly Dictionary<long, IVfx> effectsCache = new();

        protected override void OnInit(IApp app)
        {
        }

        public void PlayAtPoint(IVfx vfx, Vector3 point, Vector3 forward, AudioClip sfx = null, Action done = null)
        {
            if (effects.ContainsKey(vfx) == false)
            {
                effects.Add(vfx, VfxPool.Create(vfx));
            }

            var ps = effects[vfx].Play(point);
            
            ps.Transform.forward = forward;
            
            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                app.Coroutine.Begin(WaitForEffect(ps, done));
            }
        }

        public long Play(IVfx vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null, Vector3 offset = default, Action done = null)
        {
            if (effects.ContainsKey(vfx) == false)
            {
                effects.Add(vfx, VfxPool.Create(vfx));
            }

            IVfx ps;
            
            if (parent != null)
            {
                ps = effects[vfx].Play(parent);
            }
            else
            {
                ps = effects[vfx].Play();
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
                app.Coroutine.Begin(WaitForEffect(ps, done));
            }

            return h;
        }

        public void Stop(IVfx vfx, long hash)
        {
            if (effects.ContainsKey(vfx) == false)
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
            if (effects.ContainsKey(vfx) == false)
            {
                return;
            }

            effects[vfx].StopAll();
        }

        private IEnumerator WaitForEffect(IVfx vfx, Action done)
        {
            yield return new WaitUntil(vfx.IsAlive);
            
            done?.Invoke();
        }

        public void Process(in float delta)
        {
            var itemsToRemove = new List<long>();
            
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
