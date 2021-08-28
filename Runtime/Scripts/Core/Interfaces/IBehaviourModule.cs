namespace DosinisSDK.Core
{
    public interface IBehaviourModule : IModule
    {
        int InitOrder { get; }
    }

}
