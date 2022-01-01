using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ISceneManager : IBehaviourModule
    {
        T As<T>() where T : class, ISceneManager;
    }
}


