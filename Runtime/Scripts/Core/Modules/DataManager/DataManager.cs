using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class DataManager : Module, IDataManager
    {
        private DataManagerConfig config;
        private readonly Dictionary<string, object> dataCache = new();

        // ReSharper disable once InconsistentNaming
        private static readonly string EDITOR_SAVE_PATH = Path.Combine(Application.dataPath, "Saves");
        private const string DATA_WIPE_SAVE_KEY = "data_wipe";

        private static string loadedSaveSlot = "";
        private IApp app;

        public bool DataWipeDetected { get; private set; }

        protected override void OnInit(IApp app)
        {
            this.app = app;

            config = GetConfigAs<DataManagerConfig>();
            
#if UNITY_EDITOR
            if (Directory.Exists(EDITOR_SAVE_PATH) == false)
            {
                DeleteAllData();
                Directory.CreateDirectory(EDITOR_SAVE_PATH);
            }
#endif
            if (config)
            {
                if (config.ForceWipeOnStartup)
                {
                    DeleteAllData();
                    DataWipeDetected = true;
                    Warn("Data wipe forced by config, all data was deleted");
                }
                else
                {
                    var wipeId = PlayerPrefs.GetInt(DATA_WIPE_SAVE_KEY);
                
                    if (wipeId != config.WipeVersion)
                    {
                        DeleteAllData();
                    
                        DataWipeDetected = true;
                        Warn("Automatic data wipe detected, all data was deleted");
                    }
                }
            }
            
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
            foreach (var data in dataCache)
            {
                SaveRawData(data.Value, data.Value.GetType());
            }
            
            PlayerPrefs.Save();
        }

        private void SaveRawData(object data, Type type)
        {
            string json = JsonConvert.SerializeObject(data);
            var saveKey = BuildSaveKey(type);
            PlayerPrefs.SetString(saveKey, json);
#if UNITY_EDITOR
            File.WriteAllText(GetEditorSavePath(saveKey), json);
#endif
        }
        
        private string GetEditorSavePath(string key)
        {
            return Path.Combine(EDITOR_SAVE_PATH, key + ".json");
        }

        public T GetOrCreateData<T>() where T : class, IData, new()
        {
            if (dataCache.TryGetValue(typeof(T).Name, out object cached))
            {
                return cached as T;
            }
            
            if (HasData<T>())
            {
                var loaded = LoadRawData<T>();
                dataCache[typeof(T).Name] = loaded;
                return loaded;
            }
            
            var created = new T();
            dataCache[typeof(T).Name] = created;
            return created;
        }

        public T LoadRawData<T>() where T : class, IData, new()
        {
            string saveKey = BuildSaveKey<T>();

            string json = "";

            if (HasData<T>())
            {
                // ReSharper disable once RedundantAssignment
                json = PlayerPrefs.GetString(saveKey);
            }

#if UNITY_EDITOR
            if (File.Exists(GetEditorSavePath(saveKey)))
            {
                json = File.ReadAllText(GetEditorSavePath(saveKey));
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
        
        /// <summary>
        /// Switches the active save slot and performs a full App restart.
        ///
        /// Intended to be called from a launcher / profile selection flow.
        ///
        /// Usage pattern:
        /// - User selects save slot in some Launcher scene
        /// - App restarts and continues boot normally with the new slot
        /// - In the Launcher scene capture that it was selected (with some static field) and boot the game instead
        ///
        /// Note:
        /// This will recreate all modules and scenes. Avoid calling it
        /// during active gameplay.
        /// </summary>
        public void LoadSaveSlot(string saveSlot)
        {
            loadedSaveSlot = saveSlot;
            dataCache.Clear();
            
            App.Core.Restart();
        }
        
        public void SaveData<T>(T data) where T : class, IData
        {
            SaveRawData(data, data.GetType());
            PlayerPrefs.Save();
        }

        public bool HasData<T>() where T : class, IData
        {
            return PlayerPrefs.HasKey(BuildSaveKey<T>());
        }

        private bool IsGlobal<T>()
        {
            return typeof(IGlobalData).IsAssignableFrom(typeof(T));
        }

        private string GetKeyFromType<T>()
        {
            return typeof(T).Name;
        }

        public void DeleteData<T>() where T : class, IData
        {
            var key = typeof(T).Name;
            dataCache.Remove(key);
            
            var saveKey = BuildSaveKey<T>();
            
            bool save = false;
            
            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);
                save = true;
            }

            if (save)
            {
                PlayerPrefs.Save();
            }
            
#if UNITY_EDITOR
            if (File.Exists(GetEditorSavePath(saveKey)))
            {
                File.Delete(GetEditorSavePath(saveKey));
            }
#endif
        }

        public void DeleteAllData()
        {
            if (config && config.WipeAllPrefs)
            {
                PlayerPrefs.DeleteAll();
                dataCache.Clear();
            }
            else
            {
                var dataTypes = FindAllIDataTypes();

                foreach (var t in dataTypes)
                {
                    var key = t.Name;
                    dataCache.Remove(key);

                    var saveKey = BuildSaveKey(t);

                    if (PlayerPrefs.HasKey(saveKey))
                    {
                        PlayerPrefs.DeleteKey(saveKey);
                    }
                }
            }

            dataCache.Clear();
            
#if UNITY_EDITOR
            if (Directory.Exists(EDITOR_SAVE_PATH))
            {
                Directory.Delete(EDITOR_SAVE_PATH, true);
            }
#endif
            
            if (config)
            {
                PlayerPrefs.SetInt(DATA_WIPE_SAVE_KEY, config.WipeVersion);
            }
            
            PlayerPrefs.Save();
        }

        private List<Type> FindAllIDataTypes()
        {
            var result = new List<Type>();
            var dataInterface = typeof(IData);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                if (types == null)
                {
                    continue;
                }

                foreach (var t in types)
                {
                    if (t == null || t.IsAbstract || !t.IsClass)
                    {
                        continue;
                    }

                    if (dataInterface.IsAssignableFrom(t))
                    {
                        result.Add(t);
                    }
                }
            }

            return result;
        }

        private string BuildSaveKey(Type type)
        {
            bool isGlobal = typeof(IGlobalData).IsAssignableFrom(type);
            
            if (string.IsNullOrEmpty(loadedSaveSlot) || isGlobal)
            {
                return type.Name;
            }

            return $"{loadedSaveSlot}_{type.Name}";
        }

        private string BuildSaveKey<T>()
        {
            if (string.IsNullOrEmpty(loadedSaveSlot) || IsGlobal<T>()) 
            {
                return GetKeyFromType<T>();
            }
            
            return $"{loadedSaveSlot}_{GetKeyFromType<T>()}";
        }
    }
}