using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class UIManager : MonoBehaviour
    {
        private readonly Dictionary<Type, Window> windows = new Dictionary<Type, Window>();

        public virtual void Init(ISceneManager sceneManager)
        {
            foreach (Window win in GetComponentsInChildren<Window>(true))
            {
                windows.Add(win.GetType(), win);
            }

            foreach (var win in windows)
            {
                win.Value.Init(this);
            }
        }

        public T GetWindow<T>() where T : Window
        {
            var wType = typeof(T);

            if (windows.TryGetValue(wType, out Window window))
            {
                return (T) window;
            }

            foreach (var w in windows)
            {
                if (w.Value is T value)
                {
                    windows.Add(wType, w.Value);
                    return value;
                }
            }

            Debug.LogError($"No Window {typeof(T).Name} is available!");
            return default;
        }
    }
}