using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.UI
{
    public class BouncyButton : Button
    {
        public float scaleRatio = 0.95f;
        public float animationDuration = 0.1f;

        private Vector3 startScale;
        
        protected override void Awake()
        {
            base.Awake();
            startScale = transform.localScale;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            StartCoroutine(PressAnimationRoutine());
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
