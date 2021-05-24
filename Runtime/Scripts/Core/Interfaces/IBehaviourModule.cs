namespace DosinisSDK.Core
{
    public interface IBehaviourModule
    {
        public void Init(IApp app);
        public void Process(float delta);
    }

}

