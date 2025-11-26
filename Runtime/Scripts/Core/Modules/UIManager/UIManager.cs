using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class UIManager : SceneModule, IUIManager, IProcessable, ITickable
    {
        [SerializeField] private bool safeMode = true;
        
        public Camera Camera { get; private set; }

        private readonly Dictionary<Type, IWindow> windows = new();
        private readonly List<IProcessable> processedWindows = new();
        private readonly List<ITickable> tickableWindows = new();
        private IApp app;

        protected override void OnInit(IApp app)
        {
            this.app = app;
            
            Camera = GetComponentInChildren<Camera>();
            
            foreach (var win in GetComponentsInChildren<IWindow>(true))
            {
                RegisterWindow(win, false);
            }

            foreach (var win in windows)
            {
                InitWindow(win.Value);
            }
        }

        private void InitWindow(IWindow window)
        {
            if (safeMode)
            {
                try
                {
                    window.Init(app);
                }
                catch (Exception ex)
                {
                    LogError($"Error while initializing {window.GetType().Name}! {ex.Message} \n {ex.StackTrace}. You can disable safe mode in UIManager component.");
                }
            }
            else
            {
                window.Init(app);
            }
            
            if (window is IProcessable proc)
            {
                processedWindows.Add(proc);
            }

            if (window is ITickable tickable)
            {
                tickableWindows.Add(tickable);
            }
        }

        private void OnDestroy()
        {
            foreach (var win in windows)
            {
                if (safeMode)
                {
                    try
                    {
                        win.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error while disposing {win.Value.GetType().Name}! {ex.Message} \n {ex.StackTrace}");
                    }
                }
                else
                {
                    win.Value.Dispose();
                }
            }
        }

        public Canvas GetCanvas(RenderMode renderMode)
        {
            var canvases = GetComponentsInChildren<Canvas>();
            foreach (var canvas in canvases)
            {
                if (canvas.renderMode == renderMode)
                {
                    return canvas;
                }
            }
            
            LogError($"No Canvas with RenderMode {renderMode} is available!");
            return canvases[0];
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

            LogError($"No Window {wType.Name} is available!");
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

        public async Task WaitForWindowAsync<T>() where T : IWindow
        {
            while (IsWindowReady<T>() == false)
            {
                await Task.Yield();
            }
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

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public void ShowWindow<T>(Action callBack = null, Action onHidden = null, Action onBeforeHide = null) where T : IWindow
        {
            var window = GetWindow<T>();

            if (window.Initialized == false)
            {
                InitWindow(window);
            }

            window.Show(callBack, onHidden, onBeforeHide);
        }

        public void ShowWindow<T>() where T : IWindow
        {
            ShowWindow<T>(null);
        }

        public void ShowWindowWithArgs<T, TArgs>(TArgs args, Action callBack = null, Action onHidden = null, Action onBeforeHide = null)
            where T : IWindowWithArgs<TArgs>
        {
            var window = GetWindow<T>() as IWindowWithArgs<TArgs>;

            if (window.Initialized == false)
            {
                InitWindow(window);
            }

            window.Show(args, callBack, onHidden, onBeforeHide);
        }

        public void HideWindow<T>(Action callBack) where T : IWindow
        {
            var window =  GetWindow<T>();
            window.Hide(callBack);
        }

        public void HideWindow<T>() where T : IWindow
        {
            HideWindow<T>(null);
        }

        public void HideImmediately<T>() where T : IWindow
        {
            GetWindow<T>().HideImmediately();
        }

        public bool IsWindowShown<T>() where T : IWindow
        {
            if (TryGetWindow<T>(out var window))
            {
                return window.IsShown;
            }
            
            return false;
        }

        public void RegisterWindow(IWindow window, bool initialize = true)
        {
            windows.Add(window.GetType(), window);

            if (initialize)
            {
                InitWindow(window);
            }
        }

        public bool AnyPopupShown()
        {
            foreach (var w in windows)
            {
                if (w.Value.IsPopup && w.Value.IsShown)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task WaitForPopupsHidden()
        {
            while (AnyPopupShown())
            {
                await Task.Yield();
            }
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