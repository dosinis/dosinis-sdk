using System;
using System.Collections.Generic;
using System.Linq;
using DosinisSDK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI.Elements
{
    public class InfiniteElementSet : ElementSet
    {
        [SerializeField] private float spacing = 0;
        [SerializeField] private RectTransform viewport = null;
        [SerializeField] private float anchorOffset = 0;
        [SerializeField] private bool isVertical = true;

        [SerializeField] private float paddingTop = 0;
        [SerializeField] private float paddingBottom = 0;
        [SerializeField] private float paddingLeft = 0;
        [SerializeField] private float paddingRight = 0;

        private readonly List<object> valuesCache = new();

        private float elementSize;
        private float elementVisualSize;
        private int visibleElementCount;
        private RectTransform anchor = null;

        private Element[] currentElements = Array.Empty<Element>();
        private int[] slotIndices = Array.Empty<int>();

        public RectTransform Anchor => anchor;

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
            float padStart = isVertical ? paddingTop : paddingLeft;
            float padEnd = isVertical ? paddingBottom : paddingRight;
            float viewSize = isVertical ? viewport.rect.height : viewport.rect.width;

            float contentExtent = padStart + valuesCache.Count * elementSize + padEnd;
            float maxLogicalScroll = Mathf.Max(0f, contentExtent - viewSize);

            float logicalScroll = currentPosition + anchorOffset - padStart;
            if (logicalScroll < 0f) logicalScroll = 0f;
            if (logicalScroll > maxLogicalScroll) logicalScroll = maxLogicalScroll;

            int maxPivotByData = Mathf.Max(0, valuesCache.Count - visibleElementCount);
            int maxPivotByScroll = Mathf.Max(0, Mathf.FloorToInt((maxLogicalScroll) / elementSize));
            int maxPivot = Mathf.Max(maxPivotByData, maxPivotByScroll);

            int pivot = Mathf.Clamp(Mathf.FloorToInt(logicalScroll / elementSize), 0, maxPivot);

            if (slotIndices.Length != visibleElementCount)
            {
                slotIndices = new int[visibleElementCount];
                for (int k = 0; k < slotIndices.Length; k++) slotIndices[k] = -1;
            }

            for (int i = 0; i < visibleElementCount; i++)
            {
                int dataIndex = pivot + i;
                Element e = currentElements[i];

                if (dataIndex < 0 || dataIndex >= valuesCache.Count)
                {
                    if (e != null) e.Hide();
                    slotIndices[i] = -1;
                    continue;
                }

                if (slotIndices[i] != dataIndex)
                {
                    if (e is TE te)
                    {
                        T v = (T)valuesCache[dataIndex];
                        te.Setup(v);
                    }
                    slotIndices[i] = dataIndex;
                }

                if (isVertical)
                {
                    float y = -(padStart + dataIndex * elementSize);
                    e.RectTransform.anchoredPosition = new Vector2(e.RectTransform.anchoredPosition.x, y);
                }
                else
                {
                    float x = padStart + dataIndex * elementSize;
                    e.RectTransform.anchoredPosition = new Vector2(x, e.RectTransform.anchoredPosition.y);
                }

                e.Show();
            }
        }

        public Element FocusAround<T>(T value)
        {
            int index = valuesCache.IndexOf(value);
            if (index < 0)
            {
                return null;
            }

            anchor.anchoredPosition = GetTarget(value, out Element targetElement);
            return targetElement;
        }
        
        public Vector2 GetTarget<T>(T value, out Element targetElement)
        {
            targetElement = null;
            
            int index = valuesCache.IndexOf(value);
            int elementIndex = index - (Mathf.FloorToInt(index / (float)currentElements.Length) * currentElements.Length);
            
            if (index < 0)
            {
                return Vector2.zero;
            }

            targetElement = GetElement(elementIndex);

            float padStart = isVertical ? paddingTop : paddingLeft;

            return isVertical
                ? new Vector2(anchor.anchoredPosition.x, (elementSize * index + elementSize / 2 + padStart) - visibleElementCount / 2f * elementSize)
                : new Vector2(-(padStart + elementSize * index), anchor.anchoredPosition.y);
        }
        
        public override void Setup<TE, T>(IEnumerable<T> objects)
        {
            if (anchor == null)
            {
                anchor = (RectTransform)content;
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)content);

            float viewSize = isVertical ? viewport.rect.height : viewport.rect.width;

            elementVisualSize = isVertical
                ? element.RectTransform.rect.height
                : element.RectTransform.rect.width;

            elementSize = elementVisualSize + spacing;

            visibleElementCount = Mathf.CeilToInt(viewSize / elementSize) + 1;

            var enumerable = objects as T[] ?? objects.ToArray();

            float contentSize = elementSize * enumerable.Length;
            if (isVertical)
                anchor.SetHeight(contentSize + paddingTop + paddingBottom);
            else
                anchor.SetWidth(contentSize + paddingLeft + paddingRight);
            
            valuesCache.Clear();
            Clear();

            if (visibleElementCount != currentElements.Length)
            {
                currentElements = new Element[visibleElementCount];
                slotIndices = new int[visibleElementCount];
                for (int k = 0; k < slotIndices.Length; k++) slotIndices[k] = -1;
            }

            element.Hide();

            int i = 0;
            foreach (T value in enumerable)
            {
                valuesCache.Add(value);
                i++;
            }

            float padStart = isVertical ? paddingTop : paddingLeft;

            for (int j = 0; j < visibleElementCount; j++)
            {
                Element e;
                if (j < spawnedElements.Count)
                {
                    e = spawnedElements[j];
                }
                else
                {
                    e = Instantiate(element, anchor);
                    e.Init();
                    spawnedElements.Add(e);
                }

                currentElements[j] = e;
                slotIndices[j] = -1;

                if (isVertical)
                {
                    float y = -(padStart + j * elementSize);
                    e.RectTransform.anchoredPosition = new Vector2(e.RectTransform.anchoredPosition.x, y);
                }
                else
                {
                    float x = padStart + j * elementSize;
                    e.RectTransform.anchoredPosition = new Vector2(x, e.RectTransform.anchoredPosition.y);
                }

                e.Hide();
            }
        }
    }
}