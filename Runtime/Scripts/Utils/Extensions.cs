using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

namespace DosinisSDK.Utils
{
    public static class Extensions
    {
        // Collections

        public static T Random<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            return array[UnityRandom.Range(0, array.Length)];
        }

        public static T Random<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[UnityRandom.Range(0, list.Count)];
        }
        
        public static int RandomIndex<T>(this T[] array)
        {
            if (array.Length == 0)
                return 0;

            return UnityRandom.Range(0, array.Length);
        }
        
        public static int RandomIndex<T>(this List<T> list)
        {
            if (list.Count == 0)
                return 0;

            return UnityRandom.Range(0, list.Count);
        }

        public static T Last<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            return array[array.Length - 1];
        }

        public static T Last<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[list.Count - 1];
        }

        public static void Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int randomIndex = UnityRandom.Range(i, array.Length);
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
        }
        
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityRandom.Range(i, list.Count());
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        public static int Count(this IEnumerable collection)
        {
            int amount = 0;
            foreach (var element in collection)
            {
                amount++;
            }

            return amount;
        }
        
        public static bool Empty(this IEnumerable collection)
        {
            return collection.Count() == 0;
        }

        // Vectors

        public static void SetX(this ref Vector3 vector, float newX)
        {
            vector.Set(newX, vector.y, vector.z);
        }

        public static void SetY(this ref Vector3 vector, float newY)
        {
            vector.Set(vector.x, newY, vector.z);
        }

        public static void SetZ(this ref Vector3 vector, float newZ)
        {
            vector.Set(vector.x, vector.y, newZ);
        }

        public static void SetX(this ref Vector2 vector, float newX)
        {
            vector.Set(newX, vector.y);
        }

        public static void SetY(this ref Vector2 vector, float newY)
        {
            vector.Set(vector.x, newY);
        }

        // Strings

        public static string RemovePathExtension(this string path)
        {
            string ext = Path.GetExtension(path);

            if (string.IsNullOrEmpty(ext) == false)
            {
                return path.Remove(path.Length - ext.Length, ext.Length);
            }

            return path;
        }

        // Long

        /// <summary>
        /// Returns time string in such formats: 59s; 59m 59s; 24h 59m; 30d;
        /// </summary>
        /// <returns></returns>
        public static string ToTimeString(this long time)
        {
            if (time < 60) // Seconds
            {
                return $"{time}s";
            }

            if (time < 3600) // Minutes Seconds
            {
                return $"{time / 60}m {time % 60}s";
            }

            if (time < 86400) // Hours Minutes
            {
                return $"{time / 3600}h {time / 60 % 60}m";
            }

            // Days

            return $"{time / 86400}d";
        }
        
        /// <summary>
        /// Returns time string in 59; 59:59; 00:59:59; 30d format.
        /// </summary>
        /// <returns></returns>
        public static string ToDurationString(this long time)
        {
            if (time < 60) // Seconds
            {
                return $"{time:00}";
            }

            if (time < 3600) // Minutes Seconds
            {
                return $"{time / 60:00}:{time % 60:00}";
            }

            if (time < 86400) // Hours Minutes Seconds
            {
                return $"{time / 3600:00}:{time / 60 % 60:00}:{time % 60:00}";
            }

            // Days

            return $"{time / 86400}d";
        }

        // Int

        public static string ToTimeString(this int time)
        {
            return ((long)time).ToTimeString();
        }

        public static string ToDurationString(this int time)
        {
            return ((long)time).ToDurationString();
        }

        // Float

        public static string ToTimerString(this float time)
        {
            return $"{time:F2}";
        }

        public static string ToTimeString(this float time)
        {
            return ((long)time).ToTimeString();
        }
        
        public static string ToDurationString(this float time)
        {
            return ((long)time).ToDurationString();
        }
        
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

        // Rect
        
        public static Vector2 GetRandomPointInside(this Rect rect)
        {
            return new Vector2(UnityRandom.Range(rect.min.x, rect.max.x), UnityRandom.Range(rect.min.y, rect.max.y));
        }

        // Transforms

        public static void SetPosX(this Transform transform, float value)
        {
            transform.position = new Vector3(value, transform.position.y, transform.position.z);
        }

        public static void SetPosY(this Transform transform, float value)
        {
            transform.position = new Vector3(transform.position.x, value, transform.position.z);
        }

        public static void SetPosZ(this Transform transform, float value)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, value);
        }

        public static void SetLocalPosX(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
        }

        public static void SetLocalPosY(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
        }

        public static void SetLocalPosZ(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
        }
        
        // Color

        public static Color SetAlpha(ref this Color color, float alpha)
        {
            color = new Color(color.r, color.g, color.b, alpha);

            return color;
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