using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class Managed : MonoBehaviour, IManaged
    {
        [SerializeField] private bool autoInit = false;

        public ISceneManager SceneManager { get; private set; }
        public bool Alive { get; private set; }
        public bool AutoInit => autoInit;

        public void Init(ISceneManager sceneManager)
        {
            SceneManager = sceneManager;
            Alive = true;
        }

        public abstract void OnInit();

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
