using System;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IModule
    {
        event Action<(Scene oldScene, Scene newScene)> OnSceneChanged;
        event Action OnSceneAboutToChange;
        float SceneLoadProgress { get; }
        Scene ActiveScene { get; }
        void SwitchLoadedScene();
        void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, float delay = 0f,
            Action done = null);
    }
}


