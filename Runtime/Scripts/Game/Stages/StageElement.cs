using UnityEngine;

namespace DosinisSDK.Game
{
    public class StageElement : MonoBehaviour
    {
        protected Stage stage;
        public bool Alive { get; private set; }

        public virtual void Init(Stage stage)
        {
            this.stage = stage;
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
