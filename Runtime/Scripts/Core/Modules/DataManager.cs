using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class DataManager : Module, IDataManager
    {
        private Dictionary<string, object> dataRegistry = new Dictionary<string, object>();

        public override void Init(IApp app)
        {
            app.OnAppFocus += App_OnAppFocus;
            app.OnAppPaused += App_OnAppPaused;
        }

        private void App_OnAppPaused(bool paused)
        {
            if (paused)
            {
                SaveAll();
            }
        }

        private void App_OnAppFocus(bool focus)
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
            if (HasData<T>())
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

        public bool HasData<T>()
        {
            return PlayerPrefs.HasKey(typeof(T).Name);
        }
    }
}


