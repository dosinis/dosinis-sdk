using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class MeshVfx : VfxBase
    {
        [SerializeField] private MeshRenderer[] meshes;
        [SerializeField] private ParticleSystem[] particles;
        
        public override void Play()
        {
            foreach (var mesh in meshes)
            {
                mesh.enabled = true;
            }
            
            foreach (var particle in particles)
            {
                particle.Play();
            }
        }

        public override void Stop(bool withChildren = true)
        {
            foreach (var mesh in meshes)
            {
                mesh.enabled = false;
            }
            
            foreach (var particle in particles)
            {
                particle.Stop();
            }
        }

        public override bool IsAlive()
        {
            bool alive = false;
            
            foreach (var mesh in meshes)
            {
                alive |= mesh.enabled;
            }
            
            foreach (var particle in particles)
            {
                alive |= particle.isPlaying;
            }
            
            return alive;
        }

        public override bool Expired()
        {
            return gameObject == null;
        }

        public override bool IsPlaying => IsAlive();
        public override Transform Transform => transform;
        public override GameObject GameObject => gameObject;
    }
}
