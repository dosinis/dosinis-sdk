using DosinisSDK.Config;
using DosinisSDK.Model;
using UnityEngine;

namespace DosinisSDK.Core
{
    public abstract class BehaviourModule : MonoBehaviour, IBehaviourModule
    {
        [SerializeField] private int initOrder = 0;
        [SerializeField] private bool debug = false;
        [SerializeField] protected ModuleConfig mainConfig;

        public int InitOrder => initOrder;

        public abstract void Init(IApp app);

        public virtual void Process(float delta)
        {

        }

        public T GetConfigAs<T>() where T : ModuleConfig
        {
            return mainConfig as T;
        }

        protected void Log(string msg)
        {
            if (debug)
            {
                Debug.Log(msg);
            }
           
        }
    }
}
