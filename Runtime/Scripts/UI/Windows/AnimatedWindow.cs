using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AnimatedWindow : Window // TODO: make more generic and customizable - AnimatedWindow
    {
        [SerializeField] private bool showOverlay = false;

        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool useScale = false;
        [SerializeField] private AnimationCurve scaleInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve scaleOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        public override void Init(IUIManager uIManager)
        {
            base.Init(uIManager);
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        public sealed override void Show(Action done = null)
        {
            OnBeforeShow();

            if (useScale)
                rectTransform.localScale = Vector3.zero;

            canvasGroup.alpha = 0;

            if (showOverlay && !(this is OverlayWindow))
            {
                transform.SetAsLastSibling();
                OverlayWindow.ShowOverlay(this);
            }

            base.Show(done);
            StartCoroutine(FadeInRoutine());
        }

        public sealed override void Hide(Action done = null)
        {
            OnBeforeHide();

            if (showOverlay && !(this is OverlayWindow))
            {
                OverlayWindow.HideOverlay();
            }

            StartCoroutine(FadeOutRoutine(() =>
            {
                base.Hide(done);
            }));
        }

        public virtual void OnBeforeShow() 
        { 

        }

        public virtual void OnBeforeHide() 
        { 

        }

        private IEnumerator FadeInRoutine()
        {
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, fadeCurve.Evaluate(timer / fadeDuration));

                if (useScale)
                {
                    float evaluation = scaleInCurve.Evaluate(timer / fadeDuration);
                    rectTransform.localScale = new Vector3(evaluation, evaluation, evaluation);
                }

                yield return null;
            }
        }

        private IEnumerator FadeOutRoutine(Action done)
        {
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, fadeCurve.Evaluate(timer / fadeDuration));

                if (useScale)
                {
                    float evaluation = scaleOutCurve.Evaluate(timer / fadeDuration);
                    rectTransform.localScale = new Vector3(evaluation, evaluation, evaluation);
                }

                yield return null;
            }

            done();
        }
    }
}

