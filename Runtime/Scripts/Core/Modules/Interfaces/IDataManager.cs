namespace DosinisSDK.Core
{
    public interface IDataManager : IModule
    {
        void SaveData<T>(T data);
        T LoadData<T>() where T : class, new();
        bool HasData<T>();
        void RegisterData<T>(T data);
        void DeleteData<T>() where T : class, new();
    }

}

