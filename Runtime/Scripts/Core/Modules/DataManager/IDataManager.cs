namespace DosinisSDK.Core
{
    public interface IDataManager : IModule
    {
        void SaveData<T>(T data) where T : class, IData;
        void SaveAll();
        T LoadRawData<T>() where T : class, IData, new();
        T GetOrCreateData<T>() where T : class, IData, new();
        bool HasData<T>() where T : class, IData;
        void DeleteData<T>() where T : class, IData;
        void DeleteData(IData data);
        void DeleteAllData();
        bool DataWipeDetected { get; }
        void LoadSaveSlot(string saveSlot);
        void DeleteSaveSlot(string saveSlot);
    }

}

