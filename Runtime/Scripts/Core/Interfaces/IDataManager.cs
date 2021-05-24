namespace DosinisSDK.Core
{
    public interface IDataManager : IBehaviourModule
    {
        void SaveData<T>(T data);
        T LoadData<T>() where T : class, new();
        void RegisterData<T>(T data);
    }

}

