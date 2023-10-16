using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface ICoroutineManager : IModule
    {
        Coroutine Begin(IEnumerator coroutine);
        Coroutine Begin(IEnumerator coroutine, Action done);
        void Stop(ref Coroutine coroutine);
    }
}
