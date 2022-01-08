using DosinisSDK.Audio;
using DosinisSDK.Core;
using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class DefaultButtonAnimation : MonoBehaviour, IButtonAnimation
    {
        public float scaleRatio = 0.95f;
        public float animationDuration = 0.1f;

        public AudioClip clickSfx;
        private Vector3 startScale;

        private IAudioManager audioManager;

        public void Init()
        {
            startScale = transform.localScale;

            if (clickSfx && audioManager == null)
            {
                App.InitSignal(() =>
                {
                    audioManager = App.Core.GetModule<IAudioManager>();
                });
            }
        }

        public void PressAnimation(Action callback)
        {
            StartCoroutine(PressAnimationRoutine(callback));

            if (clickSfx && audioManager != null)
            {
                audioManager.PlayOneShot(clickSfx);
            }
        }

        public void ReleaseAnimation(Action callback)
        {
            StartCoroutine(BounceAnimationRoutine(callback));
        }

        private IEnumerator BounceAnimationRoutine(Action callback)
        {
            float t = 0f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(startScale * scaleRatio, startScale, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }

            transform.localScale = startScale;

            callback?.Invoke();
        }

        private IEnumerator PressAnimationRoutine(Action callback)
        {
            float t = 0f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(startScale, startScale * scaleRatio, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }

            callback?.Invoke();
        }
    }
}