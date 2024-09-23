using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class DataManager : Module, IDataManager
    {
        private DataManagerConfig config;
        private readonly Dictionary<string, object> dataCache = new();
        private List<string> registeredKeys = new();

        // ReSharper disable once InconsistentNaming
        private static readonly string EDITOR_SAVE_PATH = Path.Combine(Application.dataPath, "Saves");
        private const string REGISTERED_KEYS_KEY = "keys_registry";
        private const string DATA_WIPE_SAVE_KEY = "data_wipe";
        
        private IApp app;
        
        public bool DataWipeDetected { get; private set; }
        
        protected override void OnInit(IApp app)
        {
            this.app = app;

            config = GetConfigAs<DataManagerConfig>();

            if (PlayerPrefs.HasKey(REGISTERED_KEYS_KEY))
            {
                registeredKeys = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString(REGISTERED_KEYS_KEY));
            }

            if (config)
            {
                var wipeId = PlayerPrefs.GetInt(DATA_WIPE_SAVE_KEY, 0);

                if (wipeId != config.WipeVersion)
                {
                    DeleteAllData();
                    PlayerPrefs.SetInt(DATA_WIPE_SAVE_KEY, config.WipeVersion);
                    DataWipeDetected = true;
                    Log("Data wipe detected, all data was deleted");
                }
            }
            
#if UNITY_EDITOR
            if (Directory.Exists(EDITOR_SAVE_PATH) == false)
            {
                PlayerPrefs.DeleteAll();
                Directory.CreateDirectory(EDITOR_SAVE_PATH);
            }
#endif
            
            app.OnAppFocus += OnAppFocus;
            app.OnAppPaused += OnAppPaused;
            app.OnAppQuit += OnAppQuit;
        }

        protected override void OnDispose()
        {
            app.OnAppFocus -= OnAppFocus;
            app.OnAppPaused -= OnAppPaused;
            app.OnAppQuit -= OnAppQuit;
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
            foreach (var pair in dataCache)
            {
                SaveRawData(pair.Value, pair.Key);
            }
        }

        private void SaveRawData<T>(T data, string key)
        {
            string json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, json);
            
            if (registeredKeys.Contains(key) == false)
            {
                registeredKeys.Add(key);
                PlayerPrefs.SetString(REGISTERED_KEYS_KEY, JsonConvert.SerializeObject(registeredKeys));
            }
            
            PlayerPrefs.Save();

#if UNITY_EDITOR
            File.WriteAllText(GetEditorSavePath(key), json);
#endif
        }

        private void DeleteRawData(string key)
        {
            bool save = false;
            
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                save = true;
            }
            
            if (registeredKeys.Contains(key))
            {
                registeredKeys.Remove(key);
                PlayerPrefs.SetString(REGISTERED_KEYS_KEY, JsonConvert.SerializeObject(registeredKeys));
                save = true;
            }

            if (save)
            {
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
            if (dataCache.TryGetValue(typeof(T).Name, out object data) == false)
            {
                data = LoadRawData<T>();
                
                var key = data.GetType().Name;
                dataCache.Add(key, data);
                SaveData(data);
            }

            return data as T;
        }

        public T LoadRawData<T>() where T : class, IData, new()
        {
            string dataKey = typeof(T).Name;

            string json = "";

            if (HasData<T>())
            {
                // ReSharper disable once RedundantAssignment
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
            try
            {
                return JsonConvert.DeserializeObject<T>(json) ?? new T();
            }
            catch (Exception ex)
            {
                LogError(ex.Message + "\n" + ex.StackTrace);
            }

            return new T();
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
            var key = typeof(T).Name;
            dataCache.Remove(key);
            DeleteRawData(key);
        }

        public void DeleteAllData()
        {
            var keys = new List<string>(registeredKeys);
            
            foreach (var key in keys)
            {
                DeleteRawData(key);
            }

            dataCache.Clear();
        }
    }
}