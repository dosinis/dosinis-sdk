using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadingWindow : Window // TODO: make more generic and customizable - AnimatedWindow
    {
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

        public override void Show()
        {
            if (useScale)
                rectTransform.localScale = Vector3.zero;

            canvasGroup.alpha = 0;
            base.Show();
            StartCoroutine(FadeInRoutine());
        }

        public override void Hide()
        {
            StartCoroutine(FadeOutRoutine(() => {
                base.Hide();
            }));
        }

        public override void Hide(Action done)
        {
            StartCoroutine(FadeOutRoutine(() =>
            {
                base.Hide(done);
            }));
        }

        private IEnumerator FadeInRoutine()
        {
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

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
                timer += Time.deltaTime;

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

