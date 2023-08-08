using System;
using System.Collections;
using System.Collections.Generic;
using DosinisSDK.Core;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Particles
{
    public class ParticlesManager : BehaviourModule, IParticlesManager, IProcessable
    {
        private readonly Dictionary<ParticleSystem, PlainParticlePool> effects = new();
        
        private readonly ParticleSystem[] currentEffectsCache = new ParticleSystem[100];

        protected override void OnInit(IApp app)
        {
        }

        public void PlayAtPoint(ParticleSystem vfx, Vector3 point, AudioClip sfx = null, Action done = null)
        {
            if (effects.ContainsKey(vfx) == false)
            {
                effects.Add(vfx, PlainParticlePool.Create(vfx));
            }

            var ps = effects[vfx].Play(point);
            
            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                app.Coroutine.Begin(WaitForEffect(ps, done));
            }
        }

        public void Play(ParticleSystem vfx, bool forceKeepOrientation = true, Transform parent = null, AudioClip sfx = null, Vector3 offset = default, Action done = null)
        {
            if (effects.ContainsKey(vfx) == false)
            {
                effects.Add(vfx, PlainParticlePool.Create(vfx));
            }

            ParticleSystem ps;
            
            if (parent != null)
            {
                ps = effects[vfx].Play(parent);
            }
            else
            {
                ps = effects[vfx].Play();
            }

            ps.transform.localPosition += offset;

            if (forceKeepOrientation)
            {
                for (var i = 0; i < currentEffectsCache.Length; i++)
                {
                    if (currentEffectsCache[i] != null)
                        continue;

                    currentEffectsCache[i] = ps;
                    break;
                }
            }

            if (sfx != null)
            {
                sfx.PlayOneShotSafe();
            }
            
            if (done != null)
            {
                app.Coroutine.Begin(WaitForEffect(ps, done));
            }
        }

        // TODO: Stop works bad. Need to fix it.
        public void Stop(ParticleSystem vfx)
        {
            if (effects.ContainsKey(vfx) == false)
            {
                return;
            }

            effects[vfx].StopAll();
        }

        private IEnumerator WaitForEffect(ParticleSystem vfx, Action done)
        {
            yield return new WaitUntil(vfx.IsAlive);
            
            done?.Invoke();
        }

        public void Process(in float delta)
        {
            for (var i = 0; i < currentEffectsCache.Length; i++)
            {
                var vfx = currentEffectsCache[i];
                
                if (vfx == null)
                {
                    continue;
                }

                if (vfx.IsAlive() == false)
                {
                    currentEffectsCache[i] = null;
                    continue;
                }

                vfx.transform.rotation = Quaternion.identity;
            }
        }
    }
}
