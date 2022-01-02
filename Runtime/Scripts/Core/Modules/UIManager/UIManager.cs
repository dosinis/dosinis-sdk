using System;
using System.Collections.Generic;

namespace DosinisSDK.Core
{
    public class UIManager : BehaviourModule, IUIManager
    {
        private readonly Dictionary<Type, Window> windows = new Dictionary<Type, Window>();

        protected override void OnInit(IApp app)
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

            LogError($"No Window {typeof(T).Name} is available!");
            return default;
        }

        public void ShowWindow<T>(Action callBack = null) where T : Window
        {
            GetWindow<T>().Show(callBack);
        }

        public void HideWindow<T>(Action callBack = null) where T : Window
        {
            GetWindow<T>().Hide(callBack);
        }
    }
}