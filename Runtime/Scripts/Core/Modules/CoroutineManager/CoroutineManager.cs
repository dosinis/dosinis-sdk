using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class CoroutineManager : BehaviourModule
    {
        protected override void OnInit(IApp app)
        {
        }

        public Coroutine Begin(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void Begin(IEnumerator coroutine, ref Coroutine current)
        {
            current = StartCoroutine(coroutine);
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
