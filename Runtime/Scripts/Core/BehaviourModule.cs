using DosinisSDK.Config;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class BehaviourModule : MonoBehaviour, IBehaviourModule
    {
        public int initOrder = 0;

        [SerializeField] protected ModuleConfig mainConfig;

        public abstract void Init(IApp app);

        public virtual void Process(float delta)
        {

        }
    }
}
