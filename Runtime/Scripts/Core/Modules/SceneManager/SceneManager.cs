using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public sealed class SceneManager : BehaviourModule, ISceneManager
    {
        private AsyncOperation loadSceneOperation;

        public event Action<(Scene, Scene)> OnSceneChanged;
        public event Action OnSceneAboutToChange;
        public event Action OnSceneLoadStarted;
        public bool SceneIsLoading { get; private set; }
        public float SceneLoadProgress { get; private set; }
        public Scene ActiveScene { get; private set; }
        public int PreviousSceneIndex { get; private set; } = -1;

        protected override void OnInit(IApp app)
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        // ISceneManager

        public void SwitchLoadedScene()
        {
            loadSceneOperation.allowSceneActivation = true;
            SceneIsLoading = false;
        }

        public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, float delay = 0f, Action done = null)
        {
            OnSceneLoadStarted?.Invoke();
            PreviousSceneIndex = ActiveScene.buildIndex;
            StartCoroutine(LoadSceneCoroutine("", sceneIndex, mode, switchLoadedScene, delay, done));
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true,
            float delay = 0, Action done = null)
        {
            OnSceneLoadStarted?.Invoke();
            PreviousSceneIndex = ActiveScene.buildIndex;
           
            StartCoroutine(LoadSceneCoroutine(sceneName, -1, mode, switchLoadedScene, delay, done));
        }
        
        private IEnumerator LoadSceneCoroutine(string sceneName, int sceneIndex, LoadSceneMode mode, bool switchLoadedScene, float delay, Action done)
        {
            SceneIsLoading = true;
            SceneLoadProgress = 0;
            
            yield return new WaitForSeconds(delay / 2f);

            if (string.IsNullOrEmpty(sceneName) && sceneIndex == -1)
            {
                throw new ArgumentException("Scene name or index must be provided!");
            }
            
            if (string.IsNullOrEmpty(sceneName) == false)
            {
                loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            }
            else
            {
                loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);
            }

            loadSceneOperation.allowSceneActivation = false;
            
            while (loadSceneOperation.isDone == false && loadSceneOperation.progress < 0.9f)
            {
                yield return null;
                SceneLoadProgress = loadSceneOperation.progress;
            }
            
            SceneLoadProgress = 1;
            
            yield return new WaitForSeconds(delay / 2f);
            
            OnSceneAboutToChange?.Invoke();

            yield return new WaitForEndOfFrame();
            
            if (switchLoadedScene)
            {
                SwitchLoadedScene();
            }

            done?.Invoke();
        }
        
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            ActiveScene = newScene;
            OnSceneChanged?.Invoke((oldScene, newScene));

            SceneIsLoading = false;
            SceneLoadProgress = 0f;
        }
    }
}
