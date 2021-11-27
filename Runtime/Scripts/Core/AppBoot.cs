using UnityEngine;

namespace DosinisSDK.Core
{
    public class AppBoot : MonoBehaviour
    {
        [SerializeField] private AppConfig config;

        private void Awake()
        {
            if (App.Initialized == false)
                App.Create(config);
        }

        // A bit hacky utility for easy App initialization in every scene.
        // In order to use this util, place AppConfig in resources folder root.
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootAppEditor()
        {
            if (App.Initialized)
                return;

            var config = Resources.Load<AppConfig>("AppConfig");

            if (config == null)
            {
                Debug.LogWarning($"Consider moving AppConfig to Resources folder to use App in any scene (for Editor usage only)");
                return;
            }

            App.Create(config);
        }
    }
#endif
}
