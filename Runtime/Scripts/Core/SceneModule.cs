using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class SceneModule : BehaviourModule
    {
        [Tooltip("higher priority will be initialized first")]
        [field: SerializeField] public int InitPriority { get; private set; }
    }
}
