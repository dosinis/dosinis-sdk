namespace DosinisSDK.Core
{
    public interface IModule
    {
        internal void Init(IApp app, ModuleConfig config);
        T GetConfigAs<T>() where T : ModuleConfig;
    }
}