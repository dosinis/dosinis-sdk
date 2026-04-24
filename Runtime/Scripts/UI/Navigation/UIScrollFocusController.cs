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

            var targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, target);
            // var contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);

            float offset = 0f;

            if (horizontalScroll)
            {
                float left = targetBounds.min.x;
                float right = targetBounds.max.x;

                if (left < 0)
                    offset = left;
                else if (right > viewport.rect.width)
                    offset = right - viewport.rect.width;
                else
                    return;
            }
            else
            {
                float bottom = targetBounds.min.y;
                float top = targetBounds.max.y;

                if (top > 0)
                    offset = top;
                else if (bottom < -viewport.rect.height)
                    offset = bottom + viewport.rect.height;
                else
                    return;
            }

            var moveFrom = content.anchoredPosition;
            var moveTo = moveFrom - (horizontalScroll ? new Vector2(offset, 0) : new Vector2(0, offset));

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / scrollDuration;
                content.anchoredPosition = Vector2.Lerp(moveFrom, moveTo, t);
                await UniTask.Yield(token);
            }
        }
        private bool CheckInBounds(RectTransform bounds, RectTransform target, float spacing = 0)
        {
            var leftBounds = target.rect.xMin - bounds.rect.xMin - spacing;
            if (leftBounds < 0) return false;
            var rightBounds = bounds.rect.xMax - target.rect.xMax - spacing;
            if (rightBounds < 0) return false;
            return true;
        }
    }
}