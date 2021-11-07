using DosinisSDK.Audio;
using DosinisSDK.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.UI
{
    public sealed class BouncyButton : Button
    {
        public float scaleRatio = 0.95f;
        public float animationDuration = 0.1f;
        public AudioClip clickSfx;

        private Vector3 startScale;

        private IAudioManager audioManager;

        protected override void Awake()
        {
            base.Awake();
            startScale = transform.localScale;

            App.InitSignal(() => 
            {
                if (clickSfx)
                {
                    audioManager = App.Core.GetCachedModule<IAudioManager>();
                }
            });
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            StartCoroutine(PressAnimationRoutine());
            
            if (clickSfx && audioManager != null)
            {
                audioManager.PlayOneShot(clickSfx);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            StartCoroutine(BounceAnimationRoutine());
        }

        private IEnumerator BounceAnimationRoutine()
        {
            float t = 0f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(startScale * scaleRatio, startScale, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }

            transform.localScale = startScale;
        }

        private IEnumerator PressAnimationRoutine()
        {
            float t = 0f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(startScale, startScale * scaleRatio, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }
        }
    }    
}
