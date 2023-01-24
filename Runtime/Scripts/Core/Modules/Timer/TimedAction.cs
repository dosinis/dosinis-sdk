using UnityEngine;

namespace DosinisSDK.Core
{
    public class TimedAction : ITimedAction
    {
        private Coroutine coroutine;

        public TimedAction(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }
        
        public void Cancel()
        {
            App.Core.Coroutine.Stop(ref coroutine);
        }
    }
}
