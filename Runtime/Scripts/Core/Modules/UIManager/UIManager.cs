using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class UIManager : SceneModule, IUIManager, IProcessable, ITickable
    {
        public Camera Camera { get; private set; }

        private readonly Dictionary<Type, IWindow> windows = new();
        private readonly List<IProcessable> processedWindows = new();
        private readonly List<ITickable> tickableWindows = new();

        protected override void OnInit(IApp app)
        {
            foreach (var win in GetComponentsInChildren<IWindow>(true))
            {
                windows.Add(win.GetType(), win);
            }

            foreach (var win in windows)
            {
                win.Value.Init(app);

                if (win.Value is IProcessable proc)
                {
                    processedWindows.Add(proc);
                }

                if (win.Value is ITickable tickable)
                {
                    tickableWindows.Add(tickable);
                }
            }

            Camera = GetComponentInChildren<Camera>();
        }

        private void OnDestroy()
        {
            foreach (var win in windows)
            {
                win.Value.Dispose();
            }
        }

        public T GetWindow<T>() where T : IWindow
        {
            var wType = typeof(T);

            if (windows.TryGetValue(wType, out IWindow window))
            {
                return (T)window;
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

        public bool TryGetWindow<T>(out T window) where T : IWindow
        {
            var wType = typeof(T);

            if (windows.TryGetValue(wType, out IWindow win))
            {
                window = (T)win;
                return true;
            }

            foreach (var w in windows)
            {
                if (w.Value is T value)
                {
                    windows.Add(wType, w.Value);
                    window = value;
                    return true;
                }
            }

            window = default;
            return false;
        }

        public bool IsWindowReady<T>() where T : IWindow
        {
            var wType = typeof(T);

            if (windows.TryGetValue(wType, out IWindow _))
            {
                return true;
            }

            foreach (var w in windows)
            {
                if (w.Value is T)
                {
                    windows.Add(wType, w.Value);
                    return true;
                }
            }

            return false;
        }

        public void ShowWindow<T>(Action callBack = null, Action onHidden = null, Action onBeforeHide = null) where T : IWindow
        {
            var window = GetWindow<T>();

            if (window.Initialized == false)
                window.Init(app);

            window.Show(callBack, onHidden, onBeforeHide);
        }

        public void ShowWindowWithArgs<T, TArgs>(TArgs args, Action callBack = null, Action onHidden = null, Action onBeforeHide = null)
            where T : IWindowWithArgs<TArgs>
        {
            var window = GetWindow<T>() as IWindowWithArgs<TArgs>;

            if (window.Initialized == false)
                window.Init(app);

            window.Show(args, callBack, onHidden, onBeforeHide);
        }

        public void HideWindow<T>(Action callBack = null) where T : IWindow
        {
            GetWindow<T>().Hide(callBack);
        }

        void IProcessable.Process(in float delta)
        {
            foreach (var processed in processedWindows)
            {
                processed.Process(delta);
            }
        }

        void ITickable.Tick()
        {
            foreach (var tickable in tickableWindows)
            {
                tickable.Tick();
            }
        }
    }
}