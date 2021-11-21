using DosinisSDK.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Utils
{
    public class IntroManager : Core.SceneManager
    {
        [SerializeField] private bool prewarmShaders = false;
        [SerializeField] private int targetSceneId = 1;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;

        private AsyncOperation loadSceneOperation = null;

        public float NormalizedProgress => loadSceneOperation == null ? 0 : loadSceneOperation.progress / 0.9f;
        public event Action OnMainSceneLoaded = () => { };

        public override void Init(IApp app, ModuleConfig config = null)
        {
            base.Init(app, config);

            if (prewarmShaders)
                Shader.WarmupAllShaders();

            base.Init(app);

            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            //App.InitSignal(() => {
            //    loadedApp = true;
            //});

            //while (loadedApp == false)
            //    yield return null;

            yield return new WaitForSeconds(0.5f);

            loadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetSceneId, loadMode);

            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }

            OnMainSceneLoaded();
        }
    }
}
