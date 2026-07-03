using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private UINavigationGroupBase navigationGroup;
        private List<IUIScrollFocusElement> scrollFocusElements = new();
        private CancellationTokenSource cts = new();
        private Coroutine scrollRoutine;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!scrollRect) return;
            var initPivot = scrollRect.viewport.pivot;
            scrollRect.viewport.pivot = horizontalScroll ? new Vector2(0, initPivot.y) : new Vector2(initPivot.x, 1);
            navigationGroup = GetComponent<UINavigationGroupBase>();
        }
#endif

        private void Awake()
        {
            foreach (var element in scrollRect.content.GetComponentsInChildren<IUIScrollFocusElement>())
            {
                AddUIFocusElement(element);
            }
        }

        public void Cleanup()
        {
            scrollFocusElements.Clear();
            navigationGroup.Cleanup();
        }

        private void OnDisable()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        public void CheckAndScroll(IUIScrollFocusElement element)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var index = scrollFocusElements.FindIndex(scrollElement => scrollElement == element);
            index = Mathf.Clamp(index, 0, scrollFocusElements.Count - 1);
            ScrollToIndex(index);
        }

        public void AddUIFocusElement(IUIScrollFocusElement element)
        {
            element.InitializeController(this);
            scrollFocusElements.Add(element);
            navigationGroup.AddChild(element.RectTransform.gameObject);
        }

        public void RemoveUIFocusElement(IUIScrollFocusElement element)
        {
            scrollFocusElements.Remove(element);
            navigationGroup.RemoveChild(element.RectTransform.gameObject);
        }

        public void ScrollToIndex(int index)
        {
            Canvas.ForceUpdateCanvases();

            index = Mathf.Clamp(index, 0, scrollFocusElements.Count - 1);

            RectTransform target =
                scrollFocusElements[index].RectTransform;

            float contentWidth = scrollRect.content.rect.width;
            float viewportWidth = scrollRect.viewport.rect.width;

            // Target center inside content
            float targetCenter =
                Mathf.Abs(target.anchoredPosition.x);

            // Move so target center becomes viewport center
            float targetPosition =
                targetCenter - (viewportWidth * 0.5f);

            // Convert to normalized
            float normalized =
                Mathf.Clamp01(
                    targetPosition /
                    (contentWidth - viewportWidth)
                );

            ScrollTo(normalized);
        }

        private void ScrollTo(float target)
        {
            if (scrollRoutine != null)
                StopCoroutine(scrollRoutine);

            scrollRoutine =
                StartCoroutine(SmoothScroll(target));
        }

        private IEnumerator SmoothScroll(float target)
        {
            float start = horizontalScroll
                ? scrollRect.horizontalNormalizedPosition
                : scrollRect.verticalNormalizedPosition;

            float time = 0f;

            while (time < scrollDuration)
            {
                time += Time.deltaTime;

                float t = time / scrollDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                if (horizontalScroll)
                {
                    scrollRect.horizontalNormalizedPosition =
                        Mathf.Lerp(start, target, t);
                }
                else
                {
                    scrollRect.verticalNormalizedPosition =
                        Mathf.Lerp(start, target, t);
                }

                yield return null;
            }

            if (horizontalScroll)
            {
                scrollRect.horizontalNormalizedPosition = target;
            }
            else
            {
                scrollRect.verticalNormalizedPosition = target;
            }
        }
    }
}