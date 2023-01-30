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
        
        private readonly List<object> valuesCache = new();

        private float elementHeight;
        private int currentPivot;
        private int visibleElementCount;
        private RectTransform anchor = null;
        
        private Element[] currentElements = Array.Empty<Element>();
        private Element[] elementsCache = Array.Empty<Element>();

        public void UpdateElements<TE, T>() where TE : ElementFor<T>
        {
            if (!viewport || !content)
            {
                return;
            }

            if (valuesCache.Count < 1)
            {
                return;
            }

            int pivot = Mathf.Clamp(Mathf.CeilToInt((anchor.anchoredPosition.y + anchorOffset - elementHeight) / elementHeight), 0, valuesCache.Count - visibleElementCount);

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

                float y = elementToSwap.RectTransform.anchoredPosition.y - elementHeight * visibleElementCount;

                elementToSwap.RectTransform.anchoredPosition = new Vector2(elementToSwap.RectTransform.anchoredPosition.x, y);
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

                float y = elementToSwap.RectTransform.anchoredPosition.y + elementHeight * visibleElementCount;

                elementToSwap.RectTransform.anchoredPosition = new Vector2(elementToSwap.RectTransform.anchoredPosition.x, y);
            }
        }

        public override void Setup<TE, T>(IEnumerable<T> objects)
        {
            if (anchor == null)
            {
                anchor = (RectTransform)content;
            }
            
            elementHeight = element.RectTransform.rect.height + spacing;
            visibleElementCount = Mathf.CeilToInt(viewport.rect.height / elementHeight) + 1;
            
            var enumerable = objects as T[] ?? objects.ToArray();
            float contentHeight = elementHeight * enumerable.Length;

            anchor.SetHeight(contentHeight);
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
                    e.RectTransform.anchoredPosition = new Vector2(e.RectTransform.anchoredPosition.x, i * -elementHeight);
                }
                valuesCache.Add(value);
                i++;
            }
        }
    }
}
