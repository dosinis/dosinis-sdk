using DosinisSDK.Core;
using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DefaultWindowTransition : MonoBehaviour, IWindowTransition
    {
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool useScale = false;
        [SerializeField] private AnimationCurve scaleInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve scaleOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void ShowTransition(Action done)
        {
            if (useScale)
                rectTransform.localScale = Vector3.zero;

            canvasGroup.alpha = 0;

            StartCoroutine(FadeInRoutine(done));
        }

        public void HideTransition(Action done)
        {
            StartCoroutine(FadeOutRoutine(done));
        }

        private IEnumerator FadeInRoutine(Action done)
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

            done?.Invoke();
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

            done?.Invoke();
        }
    }
}

