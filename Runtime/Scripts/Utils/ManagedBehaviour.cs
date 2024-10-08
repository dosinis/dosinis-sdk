using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        protected bool initialized = false;
        
        private async void Start()
        {
            await App.Ready();
            OnInit(App.Core);

            initialized = true;
        }

        /// <summary>
        /// Essentially Start(), but called once/if App is initialized
        /// </summary>
        protected abstract void OnInit(IApp app);
    }
}
