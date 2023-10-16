using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class TimedAction : ITimedAction, IEnumerator
    {
        private Coroutine coroutine;
        private readonly IEnumerator enumerator;

        public TimedAction(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public ITimedAction Start()
        {
            coroutine = App.Core.Coroutine.Begin(enumerator, () =>
            {
                coroutine = null;
            });

            return this;
        }

        public void Stop()
        {
            App.Core.Coroutine.Stop(ref coroutine);
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
