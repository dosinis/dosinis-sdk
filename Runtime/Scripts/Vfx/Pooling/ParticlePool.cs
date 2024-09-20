using System.Collections.Generic;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private int prewarmSize = 1;
        [SerializeField] private ParticleSystem source;
        [SerializeField] private bool useGlobalParent = false;

        private readonly List<ParticleSystem> pool = new();

        public static GameObject Parent { get; private set; }
        
        private void Awake()
        {
            if (Parent == null)
            {
                Parent = new GameObject($"POOL-{nameof(ParticlePool)}");
            }
            
            if (source.gameObject.IsInScene())
            {
                source.gameObject.SetActive(false);
            }

            for (int i = 0; i < prewarmSize; i++)
            {
                var newParticleSys = Instantiate(source, useGlobalParent ? Parent.transform : transform);
                newParticleSys.gameObject.SetActive(false);
                pool.Add(newParticleSys);
            }
        }

        public ParticleSystem Play()
        {
            foreach (var particleSys in pool)
            {
                if (particleSys.gameObject.activeSelf == false || particleSys.IsAlive() == false)
                {
                    particleSys.gameObject.SetActive(true);
                    particleSys.Play();

                    return particleSys;
                }
            }

            var newParticleSys = Instantiate(source, useGlobalParent ? Parent.transform : transform);
            pool.Add(newParticleSys);

            newParticleSys.gameObject.SetActive(true);
            newParticleSys.Play();

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
