using System.Collections.Generic;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.Vfx
{
    public class VfxPool
    {
        private readonly IVfx source;
        private readonly List<IVfx> pool = new List<IVfx>();
        
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

        private VfxPool(IVfx source)
        {
            if (source.GameObject.IsInScene())
            {
                source.GameObject.SetActive(false);
            }
            
            this.source = source;
            
            for (int i = 0; i < PREWARM_SIZE; i++)
            {
                var newVfx = Object.Instantiate(source.GameObject, Parent.transform).GetComponent<IVfx>();
                newVfx.GameObject.SetActive(false);
                pool.Add(newVfx);
            }
        }
        
        public static VfxPool Create(IVfx source)
        {
            return new VfxPool(source);
        }

        public IVfx Play()
        {
            var vfx = Play(Parent.transform);
            
            return vfx;
        }
        
        public IVfx Play(Vector3 position)
        {
            var vfx = Play();
            vfx.GameObject.transform.position = position;

            return vfx;
        }

        public IVfx Play(Transform newParent)
        {
            CleanupPool();
            
            foreach (var vfx in pool)
            {
                if (vfx.GameObject.activeSelf == false || vfx.IsPlaying == false)
                {
                    if (vfx.Transform.parent != newParent)
                    {
                        vfx.Transform.SetParent(newParent);
                    }
                    
                    vfx.Transform.localPosition = Vector3.zero;
                    vfx.Transform.rotation = Quaternion.identity;
                    vfx.GameObject.SetActive(true);
                    
                    vfx.Play();

                    return vfx;
                }
            }

            var newVfx = Object.Instantiate(source.GameObject, newParent).GetComponent<IVfx>();
            newVfx.Transform.localPosition = Vector3.zero;
            newVfx.Transform.rotation = Quaternion.identity;
            
            pool.Add(newVfx);

            newVfx.Play();
            newVfx.GameObject.SetActive(true);

            return newVfx;
        }

        private void CleanupPool()
        {
            var toRemove = new List<IVfx>();
            
            foreach (var vfx in pool)
            {
                if (vfx == null || vfx.Expired())
                {
                    toRemove.Add(vfx);
                }
            }
            
            foreach (var vfx in toRemove)
            {
                pool.Remove(vfx);
            }
        }
        
        public void StopAll()
        {
            foreach (var vfx in pool)
            {
                vfx.Stop(true);
            }
        }
    }
}
