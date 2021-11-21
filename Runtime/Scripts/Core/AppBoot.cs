using UnityEngine;

namespace DosinisSDK.Core
{
    public class AppBoot : MonoBehaviour
    {
        [SerializeField] private AppConfig config;

        private void Awake()
        {
            App.Create(config);
        }
    }
}
