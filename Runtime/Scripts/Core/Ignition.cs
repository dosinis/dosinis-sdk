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

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            loadSceneOperation = SceneManager.LoadSceneAsync(targetSceneId, loadMode);

            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }

            OnMainSceneLoaded();
        }
    }
}
