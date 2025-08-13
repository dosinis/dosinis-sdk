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
        [SerializeField] private float fadeInDuration = 0.25f;
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float fadeOutDuration = 0.25f;
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool useScale = false;
        
        [SerializeField] private AnimationCurve scaleInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float scaleInDuration = 0.25f;
        [SerializeField] private AnimationCurve scaleOutCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private float scaleOutDuration = 0.25f;
        [SerializeField] private RectTransform scaleTarget = null;

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

            if(gameObject.activeSelf == false)
                gameObject.SetActive(true);

            if (useScale && scaleInDuration > fadeInDuration)
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
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);
            
            if (useScale && scaleOutDuration > fadeOutDuration)
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
            
            while (timer < fadeInDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(0, 1, fadeInCurve.Evaluate(timer / fadeInDuration));

                yield return null;
            }

            done?.Invoke();
        }

        private IEnumerator FadeOutRoutine(Action done)
        {
            float timer = 0;
            var initAlpha = canvasGroup.alpha;
            
            while (timer < fadeOutDuration)
            {
                timer += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(initAlpha, 0, fadeOutCurve.Evaluate(timer / fadeOutDuration));

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

