using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class TimedAction : ITimedAction, IEnumerator
    {
        private Coroutine coroutine;
        private readonly IEnumerator enumerator;
        private readonly ICoroutineManager coroutineManager;

        public TimedAction(IEnumerator enumerator, ICoroutineManager coroutineManager)
        {
            this.coroutineManager = coroutineManager;
            this.enumerator = enumerator;
        }

        public ITimedAction Start()
        {
            coroutine = coroutineManager.Begin(enumerator, () =>
            {
                coroutine = null;
            });

            return this;
        }

        public void Stop()
        {
            coroutineManager.Stop(ref coroutine);
        }
        
        // IEnumerator implementation
        
        bool IEnumerator.MoveNext()
        {
            return coroutine != null; // NOTE: keeps waiting if true
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => null;
    }
}
