using UnityEngine;

namespace DosinisSDK.Core
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        [SerializeField] private UIManager uiManager;

        public UIManager UIManager => uiManager;

        protected override void OnInit(IApp app)
        {
            uiManager?.Init(this);
        }

        public virtual void Process(float delta)
        {

        }

        public T As<T>() where T : class, ISceneManager
        {
            return this as T;
        }
    }
}
