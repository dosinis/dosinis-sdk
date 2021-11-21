using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Timer : Module, ITimer
    {
        private WaitForEndOfFrame skipFrame = new WaitForEndOfFrame();
        private ICoroutineManager coroutineManager;

        public override void Init(IApp app, ModuleConfig config = null)
        {
            base.Init(app, config);

            coroutineManager = app.Coroutine;
        }

        public void Delay(float delay, Action done)
        {
            coroutineManager.Begin(DelayCoroutine(delay, done));
        }
        
        public void SkipFrame(Action done)
        {
            coroutineManager.Begin(SkipFrameCoroutine(done));
        }

        private IEnumerator DelayCoroutine(float delay, Action done)
        {
            yield return new WaitForSeconds(delay);
            done();
        }

        private IEnumerator SkipFrameCoroutine(Action done)
        {
            yield return skipFrame;
            done();
        }
    }
}
