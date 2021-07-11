using DosinisSDK.Config;

namespace DosinisSDK.Core
{
    public interface IBehaviourModule : IModule
    {
        int InitOrder { get; }
        void Process(float delta);
        T GetConfigAs<T>() where T : ModuleConfig;
    }

}

