using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class SceneModule : BehaviourModule
    {
        [Tooltip("higher priority will be initialized first")] 
        [SerializeField] private int initPriority;
        
        public int InitPriority => initPriority;
    }
}
