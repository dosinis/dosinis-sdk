using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        [SerializeField] private bool waitForScene;

        protected bool ready;
        
        private void Awake()
        {
            App.Ready(()=>
            {
                if (waitForScene == false)
                {
                    OnInit(App.Core);
                    ready = true;
                }
                else
                {
                    App.Timer.WaitUntil(() => App.Core.IsModuleReady<ISceneManager>(), () =>
                    {
                        OnInit(App.Core);
                        ready = true;
                    });
                }
            });
        }

        /// <summary>
        /// Essentially Awake(), but called once/if App is initialized
        /// </summary>
        protected abstract void OnInit(IApp app);
    }
}
