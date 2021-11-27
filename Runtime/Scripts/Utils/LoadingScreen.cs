using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Utils
{
    public class LoadingScreen : MonoBehaviour // should be window
    {
        [SerializeField] private IntroManager introManager;
        [SerializeField] private Slider loadingBar;

        private void Update()
        {
            loadingBar.value = introManager.NormalizedProgress;
        }
    }
}
