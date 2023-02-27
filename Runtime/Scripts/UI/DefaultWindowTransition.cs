using DosinisSDK.Core;
using System;
using System.Collections;
using DosinisSDK.Inspector;
using UnityEngine;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DefaultWindowTransition : MonoBehaviour, IWindowTransition
    {
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool useScale = false;
        
        [ShowIf("useScale", true), SerializeField] 
        private AnimationCurve scaleInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [ShowIf("useScale", true), SerializeField]
        private AnimationCurve scaleOutCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [ShowIf("useScale", true), SerializeField] 
        private float scaleDuration = 0.25f;
        [ShowIf("useScale", true), SerializeField]
        private RectTransform scaleTarget = null;

        private CanvasGroup canvasGroup;

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (scaleTarget == null)
            {
                scaleTarget = GetComponent<RectTransform>();
            }

            if (useScale && scaleDuration > fadeDuration)
            {
                Debug.LogWarning("Scale duration is longer than fade duration. This may cause unexpected behaviour.");
            }
        }

        public void ShowTransition(Action done)
        {
            if (useScale)
                scaleTarget.localScale = Vector3.zero;

            canvasGroup.alpha = 0;

            if(gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);

            StartCoroutine(FadeInRoutine(done));
        }

        public void HideTransition(Action done)
        {
            if (gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);

            StartCoroutine(FadeOutRoutine(done));
        }

        private IEnumerator FadeInRoutine(Action done)
        {
            float timer = 0;

            var initAlpha = canvasGroup.alpha;
            
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(initAlpha, 1, fadeCurve.Evaluate(timer / fadeDuration));

                if (useScale)
                {
                    float evaluation = scaleInCurve.Evaluate(timer / scaleDuration);
                    scaleTarget.localScale = new Vector3(evaluation, evaluation, evaluation);
                }

                yield return null;
            }

            done?.Invoke();
        }

        private IEnumerator FadeOutRoutine(Action done)
        {
            float timer = 0;
            var initAlpha = canvasGroup.alpha;
            
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(initAlpha, 0, fadeCurve.Evaluate(timer / fadeDuration));

                if (useScale)
                {
                    float evaluation = scaleOutCurve.Evaluate(timer / scaleDuration);
                    scaleTarget.localScale = new Vector3(evaluation, evaluation, evaluation);
                }

                yield return null;
            }

            done?.Invoke();
        }
    }
}

