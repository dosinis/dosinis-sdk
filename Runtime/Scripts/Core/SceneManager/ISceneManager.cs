using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IBehaviourModule
    {
        Managed CreateGameElement(GameObject source, Vector3 position);
        Managed CreateGameElement(Managed gameElement, Vector3 position);
        void DestroyGameElement(Managed element);
        T GetSingletonOfType<T>() where T : ManagedSingleton;
    }
}


