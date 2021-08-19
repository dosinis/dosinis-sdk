using DosinisSDK.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class UIManager : BehaviourModule, IUIManager
    {
        private readonly Dictionary<Type, Window> windows = new Dictionary<Type, Window>();

        public override void Init(IApp app)
        {
            foreach (Window win in GetComponentsInChildren<Window>(true))
            {
                windows.Add(win.GetType(), win);
            }

            foreach (var win in windows)
            {
                win.Value.Init();
            }
        }

        public override void Process(float delta)
        {

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