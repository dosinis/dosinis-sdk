using System.Collections.Generic;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Particles
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private int prewarmSize = 3;
        [SerializeField] private ParticleSystem source;
        [SerializeField] private bool useGlobalParent = false;

        private readonly List<ParticleSystem> pool = new List<ParticleSystem>();

        private static GameObject parent;
        
        private void Awake()
        {
            if (parent == null)
            {
                parent = new GameObject($"POOL-{nameof(ParticlePool)}");
            }
            
            if (source.gameObject.IsInScene())
            {
                source.gameObject.SetActive(false);
            }

            for (int i = 0; i < prewarmSize; i++)
            {
                var newParticleSys = Instantiate(source, useGlobalParent ? parent.transform : transform);
                newParticleSys.gameObject.SetActive(false);
                pool.Add(newParticleSys);
            }
        }

        public ParticleSystem Play()
        {
            foreach (var particleSys in pool)
            {
                if (particleSys.gameObject.activeSelf == false || particleSys.isPlaying == false)
                {
                    particleSys.gameObject.SetActive(true);
                    particleSys.Play();

                    return particleSys;
                }
            }

            var newParticleSys = Instantiate(source, useGlobalParent ? parent.transform : transform);
            pool.Add(newParticleSys);

            newParticleSys.Play();
            newParticleSys.gameObject.SetActive(true);

            return newParticleSys;
        }
        
        public ParticleSystem Play(Vector3 position)
        {
            var ps = Play();
            ps.gameObject.transform.position = position;

            return ps;
        }
    }
}
