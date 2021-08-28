using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Core
{
    public class Timer : MonoBehaviour
    {
        private WaitForEndOfFrame skipFrame = new WaitForEndOfFrame();

        public void Delay(float delay, Action done)
        {
            StartCoroutine(DelayCoroutine(delay, done));
        }
            
        public void SkipOneFrame(Action done)
        {
            StartCoroutine(SkipFrameCoroutine(done));
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
