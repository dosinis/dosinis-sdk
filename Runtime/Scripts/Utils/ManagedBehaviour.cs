using System.Threading.Tasks;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        [SerializeField] private bool skipFrame;
        
        private async void Awake()
        {
            await App.Ready();
            
            if (skipFrame)
            {
                await Task.Delay(1); // making sure that after switching the scene all SceneModule are ready as well
            }
            
            OnInit(App.Core);
        }

        /// <summary>
        /// Essentially Awake(), but called once/if App is initialized
        /// </summary>
        protected abstract void OnInit(IApp app);
    }
}
