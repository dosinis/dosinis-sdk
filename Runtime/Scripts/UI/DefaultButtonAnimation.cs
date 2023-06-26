using System.Collections;
using DosinisSDK.Audio;
using DosinisSDK.Core;
using DosinisSDK.Inspector;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class DefaultButtonAnimation : MonoBehaviour, IButtonAnimation
    {
        [SerializeField] private float scaleRatio = 0.9f;
        [SerializeField] private float animationDuration = 0.1f;
        [SerializeField] private bool interactableAffectsColor = true;
        [ShowIf("interactableAffectsColor", true)] 
        [SerializeField] private Color notInteractiveColor = Color.gray;
        [SerializeField] private bool highlightOnMouseOver = false;
        [ShowIf("highlightOnMouseOver", true)] 
        [SerializeField] private Color highlightColor = Color.gray;
        [SerializeField] private AudioClip clickSfx;

        private Vector3 startScale;
        private Color initColor;
        private Button button;
        private IAudioManager audioManager;

        public void Init()
        {
            button = GetComponent<Button>();
            initColor = button.Image.color;
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
            if (interactableAffectsColor)
            {
                button.Image.color = value ? initColor : notInteractiveColor;
            }
        }

        public void Highlight(bool value)
        {
            if (highlightOnMouseOver)
            {
                StartCoroutine(value ? HighlightAnimationRoutine() : UnhighlightAnimationRoutine());
            }
        }

        private void OnDisable()
        {
            if (button == null)
                return;
                
            if (interactableAffectsColor)
            {
                OnInteractableStateChanged(button.Interactable);
            }
            else
            {
                button.Image.color = initColor;
            }
        }

        private IEnumerator HighlightAnimationRoutine()
        {
            float t = 0f;

            while (t < 1)
            {
                button.Image.color = Color.Lerp(initColor, highlightColor, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }
        }
        
        private IEnumerator UnhighlightAnimationRoutine()
        {
            float t = 0f;

            while (t < 1)
            {
                button.Image.color = Color.Lerp(highlightColor, initColor, t);
                t += Time.deltaTime / animationDuration;
                yield return null;
            }
        }

        private IEnumerator BounceAnimationRoutine()
        {
            float t = 0f;

            var initScale = transform.localScale;
            
            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(initScale, startScale, t);
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