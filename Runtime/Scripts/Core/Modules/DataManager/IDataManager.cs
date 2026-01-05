namespace DosinisSDK.Core
{
    public interface IDataManager : IModule
    {
        void SaveData<T>(T data);
        void SaveAll();
        T LoadRawData<T>() where T : class, IData, new();
        T GetOrCreateData<T>() where T : class, IData, new();
        bool HasData<T>();
        void DeleteData<T>() where T : class, IData, new();
        void DeleteAllData();
        bool DataWipeDetected { get; }
        void LoadSaveSlot(string saveSlot);
    }

}

