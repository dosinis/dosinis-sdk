using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public class Ignition : MonoBehaviour
    {
        [SerializeField] private bool prewarmShaders = false;
        [SerializeField] private int targetSceneId = 1;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;

        private AsyncOperation loadSceneOperation = new AsyncOperation();

        public float NormalizedProgress => loadSceneOperation.progress / 0.9f;
        public event Action OnMainSceneLoaded = () => { };

        private void Awake()
        {
            if (prewarmShaders)
                Shader.WarmupAllShaders();
        }

        private void Start()
        {
            loadSceneOperation = SceneManager.LoadSceneAsync(targetSceneId, loadMode);
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }

            OnMainSceneLoaded();
        }
    }
}
