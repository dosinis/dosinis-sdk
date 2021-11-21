using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IBehaviourModule
    {
        Managed CreateManagedElement(GameObject source, Vector3 position);
        Managed CreateManagedElement(Managed managed, Vector3 position);
        void DestroyManagedElement(Managed managed);
        T GetSingletonOfType<T>() where T : ManagedSingleton;
    }
}


