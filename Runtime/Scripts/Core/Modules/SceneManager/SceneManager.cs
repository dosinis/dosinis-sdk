using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public sealed class SceneManager : BehaviourModule, ISceneManager, IProcessable
    {
        private AsyncOperation loadSceneOperation;
        
        public event Action<Scene, Scene> OnSceneChanged;
        
        public float SceneLoadProgress { get; private set; }
        
        protected override void OnInit(IApp app)
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        // ISceneManager
        
        public void Process(float delta)
        {
        }

        public void SwitchLoadedScene()
        {
            loadSceneOperation.allowSceneActivation = true;
        }

        public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, Action done = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex, mode, switchLoadedScene, done));
        }
        
        private IEnumerator LoadSceneCoroutine(int sceneIndex, LoadSceneMode mode, bool switchLoadedScene, Action done)
        {
            SceneLoadProgress = 0;

            yield return new WaitForSeconds(0.5f);

            loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);

            loadSceneOperation.allowSceneActivation = switchLoadedScene;
            
            while (!loadSceneOperation.isDone)
            {
                yield return null;
                SceneLoadProgress = loadSceneOperation.progress;
            }

            SceneLoadProgress = 1;
            
            yield return new WaitForSeconds(0.1f);
            
            done?.Invoke();
        }
        
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            OnSceneChanged?.Invoke(oldScene, newScene);
        }
    }
}
