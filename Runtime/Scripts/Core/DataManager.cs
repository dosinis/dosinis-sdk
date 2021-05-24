using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{

    public class DataManager : BehaviourModule, IDataManager
    {
        private Dictionary<string, object> dataRegistry = new Dictionary<string, object>();

        public override void Init(IApp app)
        {

        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveAll();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                SaveAll();
            }
        }

        private void SaveAll()
        {
            foreach (var pair in dataRegistry)
            {
                SaveRawData(pair.Value, pair.Key);
            }
        }

        public void RegisterData<T>(T data)
        {
            dataRegistry.Add(typeof(T).Name, data);
        }

        public T LoadData<T>() where T : class, new()
        {
            if (PlayerPrefs.HasKey(typeof(T).Name))
            {
                return JsonUtility.FromJson<T>(PlayerPrefs.GetString(typeof(T).Name));
            }
            else
            {
                return new T();
            }
        }

        private void SaveRawData<T>(T data, string key)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public void SaveData<T>(T data)
        {
            SaveRawData(data, typeof(T).Name);
        }
    }
}


