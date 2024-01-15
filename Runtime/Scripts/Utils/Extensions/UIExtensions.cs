using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

namespace DosinisSDK.Utils
{
    public static class UIExtensions
    {
          // RectTransform

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
        
        public static void SetPosX(this RectTransform rt, float value)
        {
            rt.anchoredPosition = new Vector3(value, rt.anchoredPosition.y);
        }

        public static void SetPosY(this RectTransform rt, float value)
        {
            rt.anchoredPosition = new Vector3(rt.anchoredPosition.x, value);
        }
        
        public static float GetPosX(this RectTransform rt)
        {
            return rt.anchoredPosition.x;
        }

        public static float GetPosY(this RectTransform rt)
        {
            return rt.anchoredPosition.y;
        }
        
        public static void SetSize(this RectTransform rt, float width, float height)
        {
            rt.sizeDelta = new Vector2(width, height);
        }

        public static void SetWidth(this RectTransform rt, float width)
        {
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.x);
        }

        public static void SetHeight(this RectTransform rt, float height)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }
        
        public static float GetWidth(this RectTransform rt)
        {
            return rt.sizeDelta.x;
        }

        public static float GetHeight(this RectTransform rt)
        {
            return rt.sizeDelta.y;
        }
        
        public static bool OverflowsVertical(this RectTransform rectTransform)
        {
            return LayoutUtility.GetPreferredHeight(rectTransform) > rectTransform.rect.height;
        }
    
        public static bool OverflowsHorizontal(this RectTransform rectTransform)
        {
            return LayoutUtility.GetPreferredWidth(rectTransform) > rectTransform.rect.width;
        }

        // ScrollRect
        
        public static void SnapToChild(this ScrollRect scrollRect, RectTransform child, bool ignoreX = false, bool ignoreY = false)
        {
            Canvas.ForceUpdateCanvases();
            
            Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            
            var target = new Vector2(ignoreX ? scrollRect.content.localPosition.x : 0 - (viewportLocalPosition.x + childLocalPosition.x),
                ignoreY ? scrollRect.content.localPosition.y : 0 - (viewportLocalPosition.y + childLocalPosition.y));

            scrollRect.content.localPosition = target;
        }
        
        // Rect
        
        public static Vector2 GetRandomPointInside(this Rect rect)
        {
            return new Vector2(UnityRandom.Range(rect.min.x, rect.max.x), UnityRandom.Range(rect.min.y, rect.max.y));
        }
        
        // Images

        public static void SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            image.color = color.SetAlpha(alpha);
        }

        public static void PreserveAspectRatio(this RawImage image, float padding = 0)
        {
            float w = 0, h = 0;
            var parent = image.GetComponentInParent<RectTransform>();
            var imageTransform = image.GetComponent<RectTransform>();

            if (image.texture != null)
            {
                if (!parent)
                    return;

                padding = 1 - padding;
                float ratio = image.texture.width / (float)image.texture.height;

                var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
                if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
                {
                    bounds.size = new Vector2(bounds.height, bounds.width);
                }

                h = bounds.height * padding;
                w = h * ratio;
                if (w > bounds.width * padding)
                {
                    w = bounds.width * padding;
                    h = w / ratio;
                }
            }

            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }

        // Text

        public static void SetAlpha(this Text text, float alpha)
        {
            var color = text.color;
            text.color = color.SetAlpha(alpha);
        }
        
        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            var color = text.color;
            text.color = color.SetAlpha(alpha);
        }
    }
}
