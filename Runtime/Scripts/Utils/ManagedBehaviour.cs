using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        private async void Start()
        {
            await App.Ready();
            OnInit(App.Core);
        }

        /// <summary>
        /// Essentially Awake(), but called once/if App is initialized
        /// </summary>
        protected abstract void OnInit(IApp app);
    }
}
