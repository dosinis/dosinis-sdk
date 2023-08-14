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
            particleSystem.Play();
        }

        public override void Stop(bool withChildren = true)
        {
            particleSystem.Stop(withChildren);
        }

        public override bool IsAlive()
        {
            return particleSystem.IsAlive();
        }
    }
}
