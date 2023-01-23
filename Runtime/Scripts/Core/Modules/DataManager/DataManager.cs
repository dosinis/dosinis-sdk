using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class DataManager : Module, IDataManager
    {
        private readonly Dictionary<string, object> dataRegistry = new Dictionary<string, object>();

        private readonly string EDITOR_SAVE_PATH = Path.Combine(Application.dataPath, "Saves");

        protected override void OnInit(IApp app)
        {
            app.OnAppFocus += OnAppFocus;
            app.OnAppPaused += OnAppPaused;
            app.OnAppQuit += OnAppQuit;

#if UNITY_EDITOR

            if (Directory.Exists(EDITOR_SAVE_PATH) == false)
            {
                PlayerPrefs.DeleteAll();
                Directory.CreateDirectory(EDITOR_SAVE_PATH);
            }
#endif
        }

        private void OnAppPaused(bool paused)
        {
            if (paused)
            {
                SaveAll();
            }
        }

        private void OnAppFocus(bool focus)
        {
            if (!focus)
            {
                SaveAll();
            }
        }

        private void OnAppQuit()
        {
            SaveAll();
        }

        public void SaveAll()
        {
            foreach (var pair in dataRegistry)
            {
                SaveRawData(pair.Value, pair.Key);
            }
        }

        private void SaveRawData<T>(T data, string key)
        {
            string json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            File.WriteAllText(GetEditorSavePath(key), json);
#endif
        }

        private void DeleteRawData(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }

#if UNITY_EDITOR
            if (File.Exists(GetEditorSavePath(key)))
            {
                File.Delete(GetEditorSavePath(key));
            }
#endif
        }

        private string GetEditorSavePath(string key)
        {
            return Path.Combine(EDITOR_SAVE_PATH, key + ".json");
        }

        public T GetOrCreateData<T>() where T : class, IData, new()
        {
            if (dataRegistry.TryGetValue(typeof(T).Name, out object data) == false)
            {
                data = LoadRawData<T>();
                RegisterData(data);
            }

            return data as T;
        }

        public void RegisterData<T>(T data)
        {
            dataRegistry.Add(data.GetType().Name, data);
            SaveData(data);
        }

        public T LoadRawData<T>() where T : class, IData, new()
        {
            string dataKey = typeof(T).Name;

            string json = "";

            if (HasData<T>())
            {
                json = PlayerPrefs.GetString(dataKey);
            }

#if UNITY_EDITOR
            if (File.Exists(GetEditorSavePath(dataKey)))
            {
                json = File.ReadAllText(GetEditorSavePath(dataKey));
            }
            else
            {
                return new T();
            }
#endif
            return JsonConvert.DeserializeObject<T>(json) ?? new T();
        }

        public void SaveData<T>(T data)
        {
            SaveRawData(data, data.GetType().Name);
        }

        public bool HasData<T>()
        {
            return PlayerPrefs.HasKey(typeof(T).Name);
        }

        public void DeleteData<T>() where T : class, IData, new()
        {
            DeleteRawData(typeof(T).Name);
        }
    }
}
