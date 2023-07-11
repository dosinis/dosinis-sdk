using System;
using System.Collections;
using System.Collections.Generic;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace DosinisSDK.Utils
{
    public static class Helper
    {
        /// <summary>
        /// Random roll chance. Input is between 0 and 1 or 0 and 100 (if useHundreds is true)
        /// </summary>
        /// <returns></returns>
        public static bool RollChance(float chance, bool useHundreds = false)
        {
            if (chance == 0)
                return false;

            if (useHundreds)
            {
                return Random.Range(0f, 100f) <= chance;
            }

            return Random.Range(0f, 1f) <= chance;
        }

        /// <summary>
        /// Random roll chance. Input is between 0 and 100
        /// </summary>
        /// <returns></returns>
        public static bool RollChance(int chance)
        {
            return Random.Range(1, 101) <= chance;
        }

        public static T GetRandomEnum<T>() where T : struct, Enum
        {
            return Enum.Parse<T>(Random.Range(0, Enum.GetNames(typeof(T)).Length).ToString());
        }
        
        /// <summary>
        /// Returns the same angle from eulerAngles, that is displayed in the inspector
        /// </summary>
        /// <param name="angle">initial euler angle</param>
        /// <returns></returns>
        public static float TranslateEulerAngles(float angle)
        {
            angle %= 360;

            if (angle > 180)
                return angle - 360;

            return angle;
        }

        /// <summary>
        /// Returns -1 when targetPoint is to the left, 1 to the right
        /// </summary>
        /// <returns></returns>
        public static int GetTargetDirection(Transform transform, Vector3 targetPosition)
        {
            var targetDir = targetPosition - transform.position;
            var dir = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
            
            if (dir > -1)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Returns uid of such length and format '4a9d0f06'
        /// </summary>
        /// <returns></returns>
        public static string ShortUid()
        {
            return Random.Range(int.MinValue, int.MaxValue).ToString("x");
        }

        /// <summary>
        /// Returns uid of such length and format '4a9d0f064a9d0f06'
        /// </summary>
        /// <returns></returns>
        public static string Uid()
        {
            return Random.Range(int.MinValue, int.MaxValue).ToString("x") +
                   Random.Range(int.MinValue, int.MaxValue).ToString("x");
        }

        /// <summary>
        /// Sets UI element position to mouse position
        /// </summary>
        /// <param name="element">target UI element</param>
        /// <param name="camera">Camera that is used in canvas</param>
        public static void SetElementPosToMouse(Transform element, Camera camera)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(element.parent as RectTransform, 
                    Input.mousePosition, camera, out var localPoint))
            {
                element.localPosition = localPoint;
            }
        }

        /// <summary>
        /// Checks if mouse or finger is over UI element
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUI()
        {
            if (Application.isEditor == false && Input.touchSupported)
            {
                if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return true;
                }
            }

            return EventSystem.current.IsPointerOverGameObject();
        }
        
        /// <summary>
        /// Checks if an certain point is currently outside the camera's field of view
        /// </summary>
        public static bool IsInCameraFieldOfView(Vector3 position, Camera mainCamera = null)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera == null)
            {
                throw new Exception("No main camera found. Pass camera as a parameter");
            }
            
            var viewportPoint = mainCamera.WorldToViewportPoint(position);
            
            if (viewportPoint.x < 0 || viewportPoint.x > 1 ||
                viewportPoint.y < 0 || viewportPoint.y > 1 ||
                viewportPoint.z < 0)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Returns string modifier for numbers. For example 1000 -> 1K, 1000000 -> 1M, 1000000000 -> 1B, etc.
        /// </summary>
        /// <param name="numberOfThousands"></param>
        /// <returns></returns>
        public static string GetStringModifier(int numberOfThousands)
        {
            string res;

            switch (numberOfThousands)
            {
                case 2:
                    res = "K";
                    break;

                case 3:
                    res = "M";
                    break;

                case 4:
                    res = "B";
                    break;

                case 5:
                    res = "T";
                    break;

                default:
                    char firstLetter = (char)((numberOfThousands - 6) / 26 + 'a');
                    char secondLetter = (char)((numberOfThousands - 6) % 26 + 'a');
                    res = firstLetter + secondLetter.ToString();
                    break;
            }

            return res;
        }

        /// <summary>
        /// Converts Dictionary to json. Useful for analytics data.
        /// NOTE: collection nesting support is up to one layer only.
        /// </summary>
        /// <returns></returns>
        public static string ConvertDictionaryToJson(Dictionary<string, object> dictionary)
        {
            string json = $"{{";

            int size = dictionary.Count;
            int i = 0;

            foreach (var pair in dictionary)
            {
                if (pair.Value is IEnumerable && pair.Value is string == false)
                {
                    string collectionJson = "";

                    var collection = (IEnumerable)pair.Value;

                    var colSize = collection.Count();
                    int x = 0;

                    foreach (var element in collection)
                    {
                        if (colSize <= x + 1)
                        {
                            collectionJson += $"{element}";
                        }
                        else
                        {
                            collectionJson += $"{element},";
                        }

                        x++;
                    }

                    if (size <= i + 1)
                    {
                        json += $"\"{pair.Key}\": \"{collectionJson}\"";
                    }
                    else
                    {
                        json += $"\"{pair.Key}\": \"{collectionJson}\", ";
                    }
                }
                else if (double.TryParse(pair.Value.ToString(), out double result))
                {
                    if (size <= i + 1)
                    {
                        json += $"\"{pair.Key}\": {result}";
                    }
                    else
                    {
                        json += $"\"{pair.Key}\": {result}, ";
                    }
                }
                else
                {
                    if (size <= i + 1)
                    {
                        json += $"\"{pair.Key}\": \"{pair.Value}\"";
                    }
                    else
                    {
                        json += $"\"{pair.Key}\": \"{pair.Value}\", ";
                    }
                }

                i++;
            }

            json += $"}}";

            return json;
        }
    }
}