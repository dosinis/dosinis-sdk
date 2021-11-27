using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IBehaviourModule
    {
        T As<T>() where T : class, ISceneManager;
        T GetManaged<T>() where T : class, IManaged;
        Managed CreateManagedElement(GameObject source, Vector3 position);
        Managed CreateManagedElement(Managed managed, Vector3 position);
        void DestroyManagedElement(Managed managed);
    }
}


