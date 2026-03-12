using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollFocusController : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float scrollDuration = 0.3f; // тривалість скролу
        private CancellationTokenSource cts = new();
        [SerializeField] private UIScrollFocusElement[] focusElements;

        private void Awake()
        {
            foreach (var element in focusElements)
            {
                element.InitializeController(this);
            }
        }

        private void OnDisable()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        public void CheckAndScroll(Transform target)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            ScrollTo(target, cts.Token).Forget();
        }

        private async UniTask ScrollTo(Transform target, CancellationToken token)
        {
            if (scrollRect == null || target == null)
                return;

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            Vector2 contentPos = content.anchoredPosition;
            Vector2 viewportLocalPos = viewport.InverseTransformPoint(viewport.position);
            Vector2 targetLocalPos = content.InverseTransformPoint(target.position);

            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;

            if (contentHeight <= viewportHeight)
                return;

            float targetY = targetLocalPos.y;
            float normalizedPos = 1 - Mathf.Clamp01((targetY - viewportHeight / 2) / (contentHeight - viewportHeight));

            float start = scrollRect.verticalNormalizedPosition;
            float end = normalizedPos;

            float elapsed = 0f;

            while (elapsed < scrollDuration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                scrollRect.verticalNormalizedPosition =
                    Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, elapsed / scrollDuration));

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            scrollRect.verticalNormalizedPosition = end;
        }
    }
}