using System;
using UnityEngine.SceneManagement;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IModule
    {
        event Action<Scene, Scene> OnSceneChanged; // oldScene, newScene
        float SceneLoadProgress { get; }
        void SwitchLoadedScene();
        void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, bool switchLoadedScene = true,
            Action done = null);
    }
}


