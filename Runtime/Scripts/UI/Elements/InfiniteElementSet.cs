using System;
using System.Collections.Generic;
using System.Linq;
using DosinisSDK.Utils;
using UnityEngine;

namespace DosinisSDK.UI.Elements
{
    public class InfiniteElementSet : ElementSet
    {
        [SerializeField] private float spacing = 0;
        [SerializeField] private RectTransform viewport = null;
        [SerializeField] private float anchorOffset = 0;
        [SerializeField] private bool isVertical = true;

        private readonly List<object> valuesCache = new();

        private float elementSize;
        private int currentPivot;
        private int visibleElementCount;
        private RectTransform anchor = null;

        private Element[] currentElements = Array.Empty<Element>();
        private Element[] elementsCache = Array.Empty<Element>();

        public void ProcessElements<TE, T>() where TE : ElementFor<T>
        {
            if (!viewport || !content)
            {
                return;
            }

            if (valuesCache.Count < 1)
            {
                return;
            }

            float currentPosition = isVertical ? anchor.anchoredPosition.y : Mathf.Abs(anchor.anchoredPosition.x);
            int pivot = Mathf.Clamp(Mathf.CeilToInt((currentPosition + anchorOffset - elementSize) / elementSize),
                0, valuesCache.Count - visibleElementCount);

            while (pivot > currentPivot)
            {
                if (pivot + visibleElementCount - 1 >= valuesCache.Count)
                {
                    currentPivot = pivot;

                    break;
                }

                currentPivot += (int)Mathf.Sign(pivot - currentPivot);

                var current = currentElements;

                Array.Copy(currentElements, 1, elementsCache, 0, currentElements.Length - 1);

                TE elementToSwap = (TE)currentElements[0];

                elementsCache[^1] = elementToSwap;

                T value = (T)valuesCache[currentPivot + visibleElementCount - 1];

                elementToSwap.Setup(value);

                currentElements = elementsCache;

                elementsCache = current;

                float elementToSwapPosition = isVertical
                    ? -elementToSwap.RectTransform.anchoredPosition.y
                    : elementToSwap.RectTransform.anchoredPosition.x;

                float finalPosition = elementToSwapPosition + elementSize * visibleElementCount;

                elementToSwap.RectTransform.anchoredPosition = isVertical
                    ? new Vector2(elementToSwap.RectTransform.anchoredPosition.x, finalPosition)
                    : new Vector2(finalPosition, elementToSwap.RectTransform.anchoredPosition.y);
            }

            while (pivot < currentPivot)
            {
                if (pivot < 0)
                {
                    currentPivot = 0;

                    break;
                }

                currentPivot += (int)Mathf.Sign(pivot - currentPivot);

                var current = currentElements;

                Array.Copy(currentElements, 0, elementsCache, 1, currentElements.Length - 1);

                TE elementToSwap = (TE)currentElements[visibleElementCount - 1];

                elementsCache[0] = elementToSwap;

                T value = (T)valuesCache[currentPivot];

                elementToSwap.Setup(value);

                currentElements = elementsCache;

                elementsCache = current;

                float elementToSwapPosition = isVertical
                    ? -elementToSwap.RectTransform.anchoredPosition.y
                    : elementToSwap.RectTransform.anchoredPosition.x;

                float finalPosition = elementToSwapPosition - elementSize * visibleElementCount;

                elementToSwap.RectTransform.anchoredPosition = isVertical
                    ? new Vector2(elementToSwap.RectTransform.anchoredPosition.x, finalPosition)
                    : new Vector2(finalPosition, elementToSwap.RectTransform.anchoredPosition.y);
            }
        }

        public Element FocusAround<T>(T value)
        {
            int index = valuesCache.IndexOf(value);
            int elementIndex = index - (Mathf.FloorToInt(index / (float)currentElements.Length) * currentElements.Length); 
            
            if (index < 0)
            {
                return null;
            }

            anchor.anchoredPosition = isVertical
                ? new Vector2(anchor.anchoredPosition.x, (elementSize * index + elementSize
                    / 2) - visibleElementCount / 2f * elementSize)
                : new Vector2(elementSize * -index, anchor.anchoredPosition.y);
            
            return GetElement(elementIndex);
        }
        
        public override void Setup<TE, T>(IEnumerable<T> objects)
        {
            if (anchor == null)
            {
                anchor = (RectTransform)content;
            }

            float viewSize = isVertical ? viewport.rect.height : viewport.rect.width;

            elementSize = isVertical
                ? element.RectTransform.rect.height + spacing
                : element.RectTransform.rect.width + spacing;
            visibleElementCount = Mathf.CeilToInt(viewSize / elementSize) + 1;

            var enumerable = objects as T[] ?? objects.ToArray();

            float contentSize = elementSize * enumerable.Length;
            if (isVertical)
                anchor.SetHeight(contentSize);
            else
                anchor.SetWidth(contentSize);

            currentPivot = 0;
            valuesCache.Clear();
            Clear();

            if (visibleElementCount != currentElements.Length)
            {
                currentElements = new Element[visibleElementCount];
                elementsCache = new Element[visibleElementCount];
            }

            element.Hide();

            int i = 0;
            foreach (T value in enumerable)
            {
                if (i < visibleElementCount)
                {
                    Element e;
                    if (i < spawnedElements.Count)
                    {
                        e = spawnedElements[i];
                    }
                    else
                    {
                        e = Instantiate(element, anchor);
                        e.Init();
                        spawnedElements.Add(e);
                    }

                    currentElements[i] = e;

                    if (e is ElementFor<T> ef)
                    {
                        ef.Setup(value);
                    }

                    e.Show();

                    e.RectTransform.anchoredPosition = isVertical
                        ? new Vector2(e.RectTransform.anchoredPosition.x, i * -elementSize)
                        : new Vector2(i * elementSize, e.RectTransform.anchoredPosition.y);
                }

                valuesCache.Add(value);
                i++;
            }
        }
    }
}