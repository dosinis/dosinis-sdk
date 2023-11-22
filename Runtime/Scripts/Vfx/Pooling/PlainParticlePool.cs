using System.Collections.Generic;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class PlainParticlePool
    { 
        private readonly ParticleSystem source;
        private readonly List<ParticleSystem> pool = new List<ParticleSystem>();
        
        private const int PREWARM_SIZE = 1;
        
        private static GameObject parent;
        private static GameObject Parent
        {
            get
            {
                if (parent != null)
                {
                    return parent;
                }
                
                parent = new GameObject($"POOL-{nameof(VfxPool)}");

                return parent;
            }
        }

        private PlainParticlePool(ParticleSystem source)
        {
            if (source.gameObject.IsInScene())
            {
                source.gameObject.SetActive(false);
            }
            
            this.source = source;
            
            for (int i = 0; i < PREWARM_SIZE; i++)
            {
                var newParticleSys = Object.Instantiate(source, Parent.transform);
                newParticleSys.gameObject.SetActive(false);
                pool.Add(newParticleSys);
            }
        }
        
        public static PlainParticlePool Create(ParticleSystem source)
        {
            return new PlainParticlePool(source);
        }

        public ParticleSystem Play()
        {
            var ps = Play(Parent.transform);
            
            return ps;
        }
        
        public ParticleSystem Play(Vector3 position)
        {
            var ps = Play();
            ps.gameObject.transform.position = position;

            return ps;
        }

        public ParticleSystem Play(Transform newParent)
        {
            CleanupPool();
            
            foreach (var particleSys in pool)
            {
                if (particleSys.gameObject.activeSelf == false || particleSys.isPlaying == false)
                {
                    if (particleSys.transform.parent != newParent)
                    {
                        particleSys.transform.SetParent(newParent);
                    }
                    
                    particleSys.transform.localPosition = Vector3.zero;
                    particleSys.transform.rotation = Quaternion.identity;
                    particleSys.gameObject.SetActive(true);
                    
                    particleSys.Play();

                    return particleSys;
                }
            }

            var newParticleSys = Object.Instantiate(source, newParent.transform);
            newParticleSys.transform.localPosition = Vector3.zero;
            newParticleSys.transform.rotation = Quaternion.identity;
            
            pool.Add(newParticleSys);

            newParticleSys.Play();
            newParticleSys.gameObject.SetActive(true);

            return newParticleSys;
        }

        private void CleanupPool()
        {
            var toRemove = new List<ParticleSystem>();
            
            foreach (var ps in pool)
            {
                if (ps == null || ps.gameObject == null)
                {
                    toRemove.Add(ps);
                }
            }
            
            foreach (var ps in toRemove)
            {
                pool.Remove(ps);
            }
        }
        
        public void StopAll()
        {
            foreach (var p in pool)
            {
                p.Stop(true);
            }
        }
    }
}
