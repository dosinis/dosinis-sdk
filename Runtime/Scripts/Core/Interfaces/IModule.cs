namespace DosinisSDK.Core
{
    public interface IModule
    {
        void Init(IApp app, ModuleConfig config = null);
        T GetConfigAs<T>() where T : ModuleConfig;
    }
}

