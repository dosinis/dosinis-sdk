using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Utils
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Ignition ignition;
        [SerializeField] private Slider loadingBar;

        private void Update()
        {
            loadingBar.value = ignition.NormalizedProgress;
        }
    }
}
