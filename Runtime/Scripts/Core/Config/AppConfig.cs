using UnityEngine;

namespace DosinisSDK.Core
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "DosinisSDK/Configs/AppConfig")]
    public sealed class AppConfig : ScriptableObject
    {
        public bool safeMode = true;
        public BehaviourModule[] behaviourModules;
        public ModulesRegistry modulesRegistry;

        // A bit hacky utility for easy App initialization in every scene.
        // In order to use this util, place AppConfig in resources folder root.
#if UNITY_EDITOR
        private static AppConfig instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootAppEditor()
        {
            if (instance == null)
            {
                instance = Resources.Load<AppConfig>("AppConfig");
            }

            if (instance == null)
            {
                Debug.LogWarning($"Consider moving AppConfig to Resources folder to use App in any scene (for Editor usage only)");
                return;
            }

            App.Create(instance);
        }
    }
#endif
}
