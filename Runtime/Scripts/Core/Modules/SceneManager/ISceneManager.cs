using System;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IModule
    {
        event Action<(Scene oldScene, Scene newScene)> OnSceneChanged;
        event Action OnSceneAboutToChange;
        event Action<Scene> OnSceneUnloaded;
        event Action OnSceneLoadStarted;
        event Action<Scene> OnAdditiveSceneLoaded;
        bool SceneIsLoading { get; }
        float SceneLoadProgress { get; }
        Scene ActiveScene { get; }
        int PreviousSceneIndex { get; }
        void SwitchLoadedScene();
        void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, float delay = 0f,
            Action done = null);
        void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true, float delay = 0f,
            Action done = null);
        void UnloadScene(int sceneIndex, Action done = null);
    }
}


