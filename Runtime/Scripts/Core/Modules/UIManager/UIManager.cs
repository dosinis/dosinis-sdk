using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DosinisSDK.Core
{
    public class UIManager : SceneModule, IUIManager, IProcessable, ITickable
    {
        [SerializeField] private bool safeMode = true;
        [SerializeField] private bool autoCreateWindowIfNotFound = true;
        [SerializeField] private List<WindowAssetLink> windowAssets;
        
        public Camera Camera { get; private set; }

        private readonly Dictionary<Type, IWindow> windows = new();
        private readonly List<IProcessable> processedWindows = new();
        private readonly List<ITickable> tickableWindows = new();
        
        private static IApp App => Core.App.Core;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            bool changed = false;

            foreach (var link in windowAssets)
            {
                if (link != null && link.RefreshTypeIfNeeded())
                {
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
#endif
        protected override void OnInit(IApp app)
        {
            Camera = GetComponentInChildren<Camera>();

            foreach (var win in GetComponentsInChildren<IWindow>(true))
            {
                RegisterWindow(win, false);
            }

            var initList = new List<IWindow>(windows.Count);
            foreach (var kv in windows)
            {
                initList.Add(kv.Value);
            }

            foreach (var win in initList)
            {
                InitWindow(win);
            }
        }

        private void InitWindow(IWindow window)
        {
            if (safeMode)
            {
                try
                {
                    window.Init(App);
                }
                catch (Exception ex)
                {
                    LogError($"Error while initializing {window.GetType().Name}! {ex.Message} \n {ex.StackTrace}. You can disable safe mode in UIManager component.");
                }
            }
            else
            {
                window.Init(App);
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

        protected override void OnDispose()
        {
            var disposeList = new List<IWindow>(windows.Count);
            foreach (var kv in windows)
            {
                disposeList.Add(kv.Value);
            }

            foreach (var win in disposeList)
            {
                if (safeMode)
                {
                    try
                    {
                        win.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error while disposing {win.GetType().Name}! {ex.Message} \n {ex.StackTrace}");
                    }
                }
                else
                {
                    win.Dispose();
                }
            }
        }
        
        public Canvas GetCanvas(CanvasType canvasType)
        {
            var canvases = GetComponentsInChildren<CanvasTag>();
            foreach (var canvas in canvases)
            {
                if (canvas.Type == canvasType)
                {
                    return canvas.Canvas;
                }
            }
            
            return GetComponentInChildren<Canvas>();
        }

        public T GetWindow<T>() where T : IWindow
        {
            if (TryGetWindow(out T window))
            {
                return window;
            }

            LogError($"No Window {typeof(T).Name} is available!");
            return default;
        }

        public T GetOrCreateWindow<T>(CanvasType canvas = CanvasType.None) where T : IWindow
        {
            if (autoCreateWindowIfNotFound && TryGetWindow(out T window) == false)
            {
                window = CreateWindow<T>(canvas);
            }
            else
            {
                window = GetWindow<T>();
            }

            return window;
        }
        
        public T GetOrCreateReadyWindow<T>(CanvasType canvas) where T : IWindow
        {
            var window = GetOrCreateWindow<T>(canvas);
            
            if (window.Initialized == false)
            {
                InitWindow(window);
            }

            if (canvas != CanvasType.None)
            {
                ApplyToCanvas(canvas, window);
            }

            return window;
        }

        public bool TryGetWindow<T>(out T window) where T : IWindow
        {
            var wType = typeof(T);

            if (windows.TryGetValue(wType, out IWindow win))
            {
                window = (T)win;
                return true;
            }

            IWindow found = null;

            foreach (var kv in windows)
            {
                if (kv.Value is T)
                {
                    found = kv.Value;
                    break;
                }
            }

            if (found != null)
            {
                windows[wType] = found;
                window = (T)found;
                return true;
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
            return TryGetWindow<T>(out _);
        }
        
        public void ShowWindow<T>(Action shown, Action onHidden = null, Action onBeforeHide = null, 
            CanvasType canvas = CanvasType.None) where T : IWindow
        {
            var window = GetOrCreateReadyWindow<T>(canvas);
            window.Show(shown, onHidden, onBeforeHide);
        }
        
        public void ShowWindow<T>() where T : IWindow
        {
            ShowWindow<T>(null);
        }
        
        public void ShowWindow<T>(CanvasType canvas) where T : IWindow
        {
            ShowWindow<T>(null, canvas: canvas);
        }
        
        public void ShowWindowImmediately<T>(Action onHidden = null, Action onBeforeHide = null, 
            CanvasType canvas = CanvasType.None) where T : IWindow
        {
            var window = GetOrCreateReadyWindow<T>(canvas);

            window.ShowImmediately(onHidden, onBeforeHide);
        }

        public void ShowWindowWithArgs<T, TArgs>(TArgs args, Action shown = null, Action onHidden = null,
            Action onBeforeHide = null, CanvasType canvas = CanvasType.None)
            where T : IWindowWithArgs<TArgs>
        {
            var window = GetOrCreateReadyWindow<T>(canvas) as IWindowWithArgs<TArgs>;
            
            window.Show(args, shown, onHidden, onBeforeHide);
        }

        public void ShowWindowImmediatelyWithArgs<T, TArgs>(TArgs args, Action onHidden = null, 
            Action onBeforeHide = null, CanvasType canvas = CanvasType.None) where T : IWindowWithArgs<TArgs>
        {
            var window = GetOrCreateReadyWindow<T>(canvas) as IWindowWithArgs<TArgs>;
            
            window.ShowImmediately(args, onHidden, onBeforeHide);
        }

        public void HideWindow<T>(Action hidden) where T : IWindow
        {
            var window = GetWindow<T>();
            window.Hide(hidden);
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
            windows[window.GetType()] = window;

            if (initialize)
            {
                InitWindow(window);
            }
        }

        private T CreateWindow<T>(GameObject prefab, CanvasType canvas) where T : IWindow
        {
            if (prefab == null)
            {
                LogError("Prefab is null!");
                return default;
            }

            if (prefab.TryGetComponent(out T _) == false)
            {
                LogError($"Prefab contains no {nameof(T)} component");
                return default;
            }

            if (canvas == CanvasType.None)
            {
                canvas = CanvasType.Main;
                Warn("Specify canvas. Falling back to main canvas!");
            }

            var root = GetCanvas(canvas);

            var windowObj = Instantiate(prefab, root.transform);

            var window = windowObj.GetComponent<T>();

            if (window is MonoBehaviour mono)
            {
                mono.gameObject.SetActive(false);
            }

            RegisterWindow(window);

            return window;
        }
        
        public async Task<T> CreateWindowAsync<T>(CanvasType canvas, AssetLink link = null) where T : IWindow
        {
            var assetLink = link ?? FindAssetLink<T>();
            
            var prefab = await assetLink.GetAssetAsync<GameObject>();

            return CreateWindow<T>(prefab, canvas);
        }

        public T CreateWindow<T>(CanvasType canvas, AssetLink link = null) where T : IWindow
        {
            var assetLink = link ?? FindAssetLink<T>();

            var prefab = assetLink.GetAsset<GameObject>();

            return CreateWindow<T>(prefab, canvas);
        }
        
        private void ApplyToCanvas(CanvasType canvas, IWindow window)
        {
            var root = GetCanvas(canvas);

            if (window is not MonoBehaviour mono)
                return;

            if (mono.transform.parent == root.transform)
            {
                return;
            }
            
            mono.transform.SetParent(root.transform);
            mono.transform.localPosition = Vector3.zero;
            mono.transform.localScale = Vector3.one;
        }

        private AssetLink FindAssetLink<T>() where T : IWindow
        {
            var lookupType = typeof(T);

            AssetLink fallback = null;

            foreach (var a in windowAssets)
            {
                var windowType = a.WindowType;
                if (windowType == null)
                    continue;
                
                if (windowType == lookupType)
                {
                    return a.Asset;
                }
                
                if (fallback == null && lookupType.IsAssignableFrom(windowType))
                {
                    fallback = a.Asset;
                }
            }

            if (fallback != null)
                return fallback;

            LogError($"Failed to find {lookupType.Name} in asset links!");
            return null;
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
        
        [Serializable]
        private class WindowAssetLink
        {
            [SerializeField] private AssetLink asset;
            [SerializeField] private string windowTypeName;

#if UNITY_EDITOR
            [SerializeField] private string cachedAssetPath;
#endif

            public AssetLink Asset => asset;

            public Type WindowType
            {
                get
                {
                    if (string.IsNullOrEmpty(windowTypeName))
                        return null;

                    return Type.GetType(windowTypeName);
                }
            }

#if UNITY_EDITOR
            public bool RefreshTypeIfNeeded()
            {
                if (asset == null)
                    return false;

                var path = asset.Path;

                if (path == cachedAssetPath)
                    return false;

                cachedAssetPath = path;

                var go = asset.GetAsset<GameObject>();

                if (go == null)
                    return false;

                var window = go.GetComponent<IWindow>();

                if (window == null)
                {
                    Debug.LogError($"{go.name} does not contain IWindow");
                    return false;
                }

                windowTypeName = window.GetType().AssemblyQualifiedName;

                return true;
            }
#endif
            
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(WindowAssetLink))]
            public class WindowAssetLinkDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    var assetProp = property.FindPropertyRelative("asset");

                    EditorGUI.PropertyField(position, assetProp, label);
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("asset"), label);
                }
            }
#endif
        }
    }
}