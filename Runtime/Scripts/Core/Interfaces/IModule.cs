namespace DosinisSDK.Core
{
    public interface IModule
    {
        void Init(IApp app, ModuleConfig config);
        T GetConfigAs<T>() where T : ModuleConfig;
    }
}