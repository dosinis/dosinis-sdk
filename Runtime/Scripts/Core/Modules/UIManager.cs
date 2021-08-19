using DosinisSDK.UI;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class UIManager : BehaviourModule, IUIManager
    {
        private List<Window> windows = new List<Window>();

        public override void Init(IApp app)
        {

            foreach (Window win in GetComponentsInChildren<Window>(true))
            {
                windows.Add(win);
            }

            foreach (Window win in windows)
            {
                win.Init();
            }
        }

        public override void Process(float delta)
        {

        }

        public T GetWindow<T>() where T : Window
        {
            foreach (var win in windows)
            {
                if (win is T value)
                {
                    return value;
                }
            }

            Debug.LogError($"No Window {typeof(T).Name} is available!");
            return default;
        }

    }
}