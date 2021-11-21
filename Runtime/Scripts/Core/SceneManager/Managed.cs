using UnityEngine;

namespace DosinisSDK.Core
{
    public class Managed : MonoBehaviour
    {
        protected ISceneManager game;

        public bool Alive { get; private set; }

        public virtual void Init(ISceneManager game)
        {
            this.game = game;
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
