using System;
using System.Collections;
using System.Collections.Generic;
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

        public static int CountFast(this IEnumerable list)
        {
            int amount = 0;
            foreach (var element in list)
            {
                amount++;
            }

            return amount;
        }

        // Vectors

        public static void SetX(this Vector3 vector, float newX)
        {
            vector.Set(newX, vector.y, vector.z);
        }

        public static void SetY(this Vector3 vector, float newY)
        {
            vector.Set(vector.x, newY, vector.z);
        }

        public static void SetZ(this Vector3 vector, float newZ)
        {
            vector.Set(vector.x, vector.y, newZ);
        }

        public static void SetX(this Vector2 vector, float newX)
        {
            vector.Set(newX, vector.y);
        }

        public static void SetY(this Vector2 vector, float newY)
        {
            vector.Set(vector.x, newY);
        }

        // Strings

        public static string RemovePathExtention(this string path)
        {
            string ext = System.IO.Path.GetExtension(path);

            if (string.IsNullOrEmpty(ext) == false)
            {
                return path.Remove(path.Length - ext.Length, ext.Length);
            }

            return path;
        }

        // Long

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
    }
}