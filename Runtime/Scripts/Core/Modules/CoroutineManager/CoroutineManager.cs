using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class CoroutineManager : BehaviourModule, ICoroutineManager
    {
        protected override void OnInit(IApp app)
        {
        }

        public Coroutine Begin(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public Coroutine Begin(IEnumerator coroutine, Action done)
        {
            return StartCoroutine(StartCoroutine(coroutine, done));
        }

        private static IEnumerator StartCoroutine(IEnumerator coroutine, Action done)
        {
            yield return coroutine;
            
            done?.Invoke();
        }

        public void Stop(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
