using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Timer : Module, ITimer
    {
        private readonly WaitForEndOfFrame skipFrame = new();
        private readonly WaitForFixedUpdate skipFixedUpdate = new();
        private ICoroutineManager coroutineManager;

        protected override void OnInit(IApp app)
        {
            coroutineManager = app.GetModule<ICoroutineManager>();
        }

        public ITimedAction Delay(float delay, Action done, bool realtime = false)
        {
            var enumerator = DelayCoroutine(delay, done, realtime);
            var action = new TimedAction(enumerator);
            
            action.Start();

            return action;
        }

        public ITimedAction CreateDelayAction(float delay, Action done, bool realtime = false)
        {
            var enumerator = DelayCoroutine(delay, done, realtime);
            var action = new TimedAction(enumerator);

            return action;
        }

        public ITimedAction Sequence(Action done = null, params ITimedAction[] actions)
        {
            var enumerator = SequenceCoroutine(done, actions);
            var action = new TimedAction(enumerator);
            
            action.Start();
            
            return action;
        }

        public ITimedAction Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f)
        {
            var enumerator = RepeatCoroutine(frequency, times, initDelay, onTick);
            var action = new TimedAction(enumerator);
            
            action.Start();
            
            return action;
        }

        public void SkipFrame(Action done)
        {
            coroutineManager.Begin(SkipFrameCoroutine(done));
        }

        public void SkipFixedUpdate(Action done)
        {
            coroutineManager.Begin(SkipFixedUpdateCoroutine(done));
        }
        
        public ITimedAction WaitUntil(Func<bool> condition, Action onComplete)
        {
            var enumerator = WaitUntilCoroutine(condition, onComplete);
            var action = new TimedAction(enumerator);
            
            action.Start();
            
            return action;
        }
        
        private static IEnumerator SequenceCoroutine(Action done, params ITimedAction[] actions)
        {
            foreach (var action in actions)
            {
                yield return action.Start();
            }
            
            done?.Invoke();
        }

        private static IEnumerator DelayCoroutine(float delay, Action done, bool realtime = false)
        {
            if (realtime)
                yield return new WaitForSecondsRealtime(delay);
            else 
                yield return new WaitForSeconds(delay);
            
            done();
        }

        private static IEnumerator RepeatCoroutine(float frequency, int times, float initDelay, Action<int> onTick)
        {
            yield return new WaitForSeconds(initDelay);

            for (int i = 0; i < times; i++)
            {
                onTick?.Invoke(i);
                yield return new WaitForSeconds(frequency);
            }
        }
        
        private IEnumerator SkipFrameCoroutine(Action done)
        {
            yield return skipFrame;
            done();
        }
        
        private IEnumerator SkipFixedUpdateCoroutine(Action done)
        {
            yield return skipFixedUpdate;
            done();
        }
        
        private static IEnumerator WaitUntilCoroutine(Func<bool> condition, Action onComplete)
        {
            yield return new WaitUntil(condition);
            onComplete?.Invoke();
        }
    }
}
