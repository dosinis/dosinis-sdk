using DosinisSDK.Config;

namespace DosinisSDK.Core
{
    public interface IBehaviourModule
    {
        public int InitOrder { get; }
        public void Init(IApp app);
        public void Process(float delta);
        T GetConfigAs<T>() where T : ModuleConfig;
    }

}

