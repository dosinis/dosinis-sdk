using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class ParticleSystemVfx : VfxBase, IVfx
    {
        public override bool IsPlaying => particleSystem.isPlaying;
        public override Transform Transform => transform;
        public override GameObject GameObject => gameObject;
        
        private new ParticleSystem particleSystem;
        
        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        public override void Play()
        {
            if (particleSystem == null)
            {
                particleSystem = GetComponent<ParticleSystem>();
            }
            
            particleSystem.Play();
        }

        public override void Stop(bool withChildren = true, bool clear = true)
        {
            if (clear)
            {
                particleSystem.Stop(withChildren, ParticleSystemStopBehavior.StopEmittingAndClear);
                return;
            }
            
            particleSystem.Stop(withChildren);
        }
        
        public override bool Expired()
        {
            return particleSystem == null || gameObject == null;
        }

        public override bool IsAlive()
        {
            return Expired() == false && particleSystem.IsAlive();
        }
    }
}
