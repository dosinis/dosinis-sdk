namespace DosinisSDK.Core
{
    public interface IDataManager : IBehaviourModule
    {
        void SaveData<T>(T data);
        T LoadData<T>();
        void RegisterData<T>(T data);
    }

}

