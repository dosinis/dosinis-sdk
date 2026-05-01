using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI.Navigation
{
    public class UIScrollFocusController : MonoBehaviour, IUIScrollFocusController
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private bool horizontalScroll = true;
        [SerializeField] private float scrollDuration = 0.3f;
        [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
        private CancellationTokenSource cts = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!scrollRect) return;
            var initPivot = scrollRect.viewport.pivot;
            scrollRect.viewport.pivot = horizontalScroll ? new Vector2(0, initPivot.y) : new Vector2(initPivot.x, 1);
        }
#endif

        private void Awake()
        {
            foreach (var element in scrollRect.content.GetComponentsInChildren<IUIScrollFocusElement>())
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

        public void CheckAndScroll(RectTransform target)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            ScrollToTarget(target, cts.Token).Forget();
        }

        private async UniTask ScrollToTarget(RectTransform target, CancellationToken token)
        {
            var content = scrollRect.content;
            var viewport = scrollRect.viewport;

            if (CheckInBounds(viewport, target)) return;
            var targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, target);
            Debug.Log($"Top {viewport.rect.yMin}, Bottom {viewport.rect.yMax}, Center {viewport.rect.center}");
            // var contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);
            float offset = 0f;

            if (horizontalScroll)
            {
                offset = targetBounds.min.x;
            }
            else
            {
                offset = targetBounds.max.y;
            }

            var moveFrom = content.anchoredPosition;
            var moveTo = moveFrom - (horizontalScroll ? new Vector2(offset, 0) : new Vector2(0, offset));

            float t = 0;
            while (t < 1)
            {
                t += Mathf.Clamp(Time.deltaTime / scrollDuration, 0, 1);
                content.anchoredPosition = Vector2.Lerp(moveFrom, moveTo, t);
                await UniTask.Yield(token);
            }
        }

        private bool CheckInBounds(RectTransform bounds, RectTransform target, float spacing = 0)
        {
            var targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(bounds, target);

            if (horizontalScroll)
            {
                var leftBounds = targetBounds.min.x - bounds.rect.xMin - spacing;
                if (leftBounds < 0) return false;
                var rightBounds = bounds.rect.xMax - targetBounds.max.x - spacing;
                if (rightBounds < 0) return false;
            }
            else
            {
                var topBounds = targetBounds.min.y - bounds.rect.yMin - spacing;
                if (topBounds < 0) return false;
                var rightBounds = bounds.rect.yMax - targetBounds.max.y - spacing;
                if (rightBounds < 0) return false;
            }

            return true;
        }
    }
}