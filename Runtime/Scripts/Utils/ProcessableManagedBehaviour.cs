using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class ProcessableManagedBehaviour : ManagedBehaviour
    {
        protected abstract override void OnInit(IApp app);
        
        private void Update()
        {
            if (initialized == false)
                return;
            
            Process(Time.deltaTime);
        }
        
        protected abstract void Process(in float deltaTime);
    }
}
