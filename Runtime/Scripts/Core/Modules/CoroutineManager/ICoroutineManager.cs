using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ICoroutineManager : IBehaviourModule
    {
        Coroutine Begin(IEnumerator coroutine);
        void Begin(IEnumerator coroutine, ref Coroutine current);
        void Stop(ref Coroutine coroutine);
    }
}

