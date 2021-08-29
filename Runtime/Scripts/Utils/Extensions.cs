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
            return array[UnityRandom.Range(0, array.Length)];
        }

        public static T Random<T>(this List<T> list)
        {
            return list[UnityRandom.Range(0, list.Count)];
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

        // Images

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
    }
}