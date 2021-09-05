using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class DataManager : Module, IDataManager
    {
        private Dictionary<string, object> dataRegistry = new Dictionary<string, object>();

        private readonly string EDITOR_SAVE_PATH = Path.Combine(Application.dataPath, "Saves");

        public override void Init(IApp app)
        {
            app.OnAppFocus += App_OnAppFocus;
            app.OnAppPaused += App_OnAppPaused;

#if UNITY_EDITOR
            bool orderedDeletion = false;

            if (Directory.Exists(EDITOR_SAVE_PATH) == false)
            {
                Directory.CreateDirectory(EDITOR_SAVE_PATH);
                orderedDeletion = true;
            }

            app.OnAppInitialized += () => 
            {
                if (orderedDeletion)
                {
                    if(TryDeleteAll())
                    {
                        app.Restart();
                    }
                }
            };
#endif
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

        private bool TryDeleteAll()
        {
            bool deleted = false;
            foreach (var pair in dataRegistry)
            {
                DeleteRawData(pair.Key);
                deleted = true;
            }

            return deleted;
        }

        private void SaveRawData<T>(T data, string key)
        {
            string json = JsonUtility.ToJson(data);
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

        public void RegisterData<T>(T data)
        {
            dataRegistry.Add(typeof(T).Name, data);
            SaveData(data);
        }

        public T LoadData<T>() where T : class, new()
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
#endif

            T data = JsonUtility.FromJson<T>(json);

            if (data != null)
            {
                return data;
            }
            else
            {
                return new T();
            }
        }

        public void SaveData<T>(T data)
        {
            SaveRawData(data, typeof(T).Name);
        }

        public bool HasData<T>()
        {
            return PlayerPrefs.HasKey(typeof(T).Name);
        }

        public void DeleteData<T>() where T : class, new()
        {
            DeleteRawData(typeof(T).Name);
        }
    }
}
