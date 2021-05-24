namespace DosinisSDK.Core
{
    public interface IApp
    {
        public T GetCachedBehaviourModule<T>() where T : class, IBehaviourModule;
    }
}