namespace DosinisSDK.Core
{
    public class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        protected override void OnInit(IApp app)
        {   
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
