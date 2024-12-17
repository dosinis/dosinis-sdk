using System;
using System.Collections;
using DosinisSDK.Core;
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
        private AnimationCurve scaleOutCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [ShowIf("useScale", true), SerializeField] 
        private float scaleOutDuration = 0.25f;
        [ShowIf("useScale", true), SerializeField] 
        private float scaleInDuration = 0.25f;
        [ShowIf("useScale", true), SerializeField]
        private RectTransform scaleTarget = null;

        private CanvasGroup canvasGroup;

        private float duration;

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (scaleTarget == null)
            {
                scaleTarget = GetComponent<RectTransform>();
            }
        }

        public void ShowTransition(Action done)
        {
            if (useScale)
                scaleTarget.localScale = Vector3.zero;

            canvasGroup.alpha = 0;

            if(gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);

            if (useScale && scaleInDuration > fadeDuration)
            {
                StartCoroutine(ScaleInRoutine(done));
                StartCoroutine(FadeInRoutine(null));
            }
            else
            {
                if (useScale)
                {
                    StartCoroutine(ScaleInRoutine(null));
                }
                
                StartCoroutine(FadeInRoutine(done));
            }
        }

        public void HideTransition(Action done)
        {
            if (gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);


            if (useScale && scaleOutDuration > fadeDuration)
            {
                StartCoroutine(ScaleOutRoutine(done));
                StartCoroutine(FadeOutRoutine(null));
            }
            else
            {
                if (useScale)
                {
                    StartCoroutine(ScaleOutRoutine(null));
                }
                
                StartCoroutine(FadeOutRoutine(done));
            }
        }

        private IEnumerator FadeInRoutine(Action done)
        {
            float timer = 0;
            
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(0, 1, fadeCurve.Evaluate(timer / fadeDuration));

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

                yield return null;
            }

            done?.Invoke();
        }
        
        private IEnumerator ScaleInRoutine(Action done)
        {
            float timer = 0;
            
            scaleTarget.localScale = Vector3.zero;
            
            while (timer < scaleInDuration)
            {
                timer += Time.unscaledDeltaTime;

                float evaluation = scaleInCurve.Evaluate(timer / scaleInDuration);
                scaleTarget.localScale = new Vector3(evaluation, evaluation, evaluation);

                yield return null;
            }

            done?.Invoke();
        }
        
        private IEnumerator ScaleOutRoutine(Action done)
        {
            float timer = 0;
            
            while (timer < scaleOutDuration)
            {
                timer += Time.unscaledDeltaTime;

                float evaluation = scaleOutCurve.Evaluate(timer / scaleOutDuration);
                scaleTarget.localScale = new Vector3(evaluation, evaluation, evaluation);

                yield return null;
            }

            done?.Invoke();
        }
    }
}

