using UnityEngine;

namespace DosinisSDK.Core
{
    public class Managed : MonoBehaviour
    {
        protected ISceneManager scene;

        public bool Alive { get; private set; }

        public virtual void Init(ISceneManager scene)
        {
            this.scene = scene;
            Alive = true;
        }

        public virtual void Process(float delta)
        {
            
        }

        public virtual void Destruct()
        {
            Alive = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
