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

        public float SceneLoadProgress { get; private set; }
        public Scene ActiveScene { get; private set; }

        protected override void OnInit(IApp app)
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        // ISceneManager

        public void SwitchLoadedScene()
        {
            loadSceneOperation.allowSceneActivation = true;
        }

        public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, float delay = 0f, Action done = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex, mode, switchLoadedScene, delay, done));
        }
        
        private IEnumerator LoadSceneCoroutine(int sceneIndex, LoadSceneMode mode, bool switchLoadedScene, float delay, Action done)
        {
            SceneLoadProgress = 0;
            
            yield return new WaitForSeconds(delay / 2f);
            
            loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);
            
            OnSceneAboutToChange?.Invoke();
            
            loadSceneOperation.allowSceneActivation = switchLoadedScene;
            
            while (loadSceneOperation.isDone == false && loadSceneOperation.progress < 0.9f)
            {
                yield return null;
                SceneLoadProgress = loadSceneOperation.progress;
            }

            SceneLoadProgress = 1;
            
            yield return new WaitForSeconds(delay / 2f);

            done?.Invoke();
        }
        
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            ActiveScene = newScene;
            OnSceneChanged?.Invoke((oldScene, newScene));
        }
    }
}
