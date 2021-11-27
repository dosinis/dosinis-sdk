namespace DosinisSDK.Core
{
    public interface IModule
    {
        void OnInit(IApp app);
        void Init(IApp app, ModuleConfig config = null);
        T GetConfigAs<T>() where T : ModuleConfig;
    }
}

