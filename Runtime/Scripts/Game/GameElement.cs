using UnityEngine;

namespace DosinisSDK.Game
{
    public class GameElement : MonoBehaviour
    {
        protected IGame game;

        public bool Alive { get; private set; }

        public virtual void Init(IGame game)
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
