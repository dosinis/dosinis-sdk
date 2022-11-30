using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Timer : Module, ITimer
    {
        private readonly WaitForEndOfFrame skipFrame = new WaitForEndOfFrame();
        private readonly WaitForFixedUpdate skipFixedUpdate = new WaitForFixedUpdate();
        private CoroutineManager coroutineManager;

        protected override void OnInit(IApp app)
        {
            coroutineManager = app.GetModule<CoroutineManager>();
        }

        public void Delay(float delay, Action done)
        {
            coroutineManager.Begin(DelayCoroutine(delay, done));
        }
        
        public void Repeat(float frequency, int times, Action<int> onTick, float initDelay = 0f, Func<bool> cancelFunc = null)
        {
            coroutineManager.Begin(RepeatCoroutine(frequency, times, initDelay, cancelFunc, onTick));
        }

        public void SkipFrame(Action done)
        {
            coroutineManager.Begin(SkipFrameCoroutine(done));
        }

        public void SkipFixedUpdate(Action done)
        {
            coroutineManager.Begin(SkipFixedUpdateCoroutine(done));
        }
        
        public void WaitUntil(Func<bool> condition, Action onComplete)
        {
            coroutineManager.Begin(WaitUntilCoroutine(condition, onComplete));
        }

        private IEnumerator DelayCoroutine(float delay, Action done)
        {
            yield return new WaitForSecondsRealtime(delay);
            done();
        }

        private IEnumerator RepeatCoroutine(float frequency, int times, float initDelay, Func<bool> cancelFunc, Action<int> onTick)
        {
            yield return new WaitForSeconds(initDelay);

            for (int i = 0; i < times; i++)
            {
                if (cancelFunc != null)
                {
                    if (cancelFunc())
                    {
                        yield break;
                    }
                }
                
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
