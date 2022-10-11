using DosinisSDK.Audio;
using DosinisSDK.Core;
using System.Collections;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class DefaultButtonAnimation : MonoBehaviour, IButtonAnimation
    {
        [SerializeField] private float scaleRatio = 0.9f;
        [SerializeField] private float animationDuration = 0.1f;
        [SerializeField] private bool interactableAffectsAlpha = false;

        [SerializeField] private AudioClip clickSfx;

        private Vector3 startScale;

        private IAudioManager audioManager;

        public void Init()
        {
            startScale = transform.localScale;

            if (clickSfx && audioManager == null)
            {
                App.Ready(() =>
                {
                    audioManager = App.Core.GetModule<IAudioManager>();
                });
            }
        }

        public void PressAnimation()
        {
            StartCoroutine(PressAnimationRoutine());

            if (clickSfx && audioManager != null)
            {
                audioManager.PlayOneShot(clickSfx);
            }
        }

        public void ReleaseAnimation()
        {
            StartCoroutine(BounceAnimationRoutine());
        }

        public void OnInteractableStateChanged(bool value)
        {
            if (interactableAffectsAlpha)
            {
                GetComponent<Button>().Image.SetAlpha(value ? 1f : 0.8f);
            }
        }

        private IEnumerator BounceAnimationRoutine()
        {
            float t = 0f;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, startScale, t);
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