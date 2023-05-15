using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        [SerializeField] private bool skipFrame;

        private IApp app;
        
        private async void Awake()
        {
            await App.Ready();
            
            if (skipFrame)
            {
                App.Core.Timer.SkipFrame(() =>
                {
                    OnInit(App.Core);
                });
                
                return;
            }

            app = App.Core;
            OnInit(app);
        }

        /// <summary>
        /// Essentially Awake(), but called once/if App is initialized
        /// </summary>
        protected abstract void OnInit(IApp app);
    }
}
