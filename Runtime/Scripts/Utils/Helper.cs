using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace DosinisSDK.Utils
{
    public static class Helper
    {
        private static Camera cam;
        
        private static Camera MainCam
        {
            get
            {
                if (cam == null)
                {
                    cam = Camera.main;
                }

                return cam;
            }
        }
        
        /// <summary>
        /// Random roll chance. Input is between 0f and 100f or 0f and 1.0f (if useHundreds is false)
        /// </summary>
        /// <returns></returns>
        public static bool RollChance(float chance, bool useHundreds = true)
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
        public static float TranslateEulerAngle(float angle)
        {
            angle %= 360;

            if (angle > 180)
                return angle - 360;

            return angle;
        }

        public static Vector3 TranslateEulerVector(Vector3 vector)
        {
            return new Vector3(TranslateEulerAngle(vector.x), TranslateEulerAngle(vector.y), TranslateEulerAngle(vector.z));
        }

        /// <summary>
        /// Get random point in annulus (ring) shaped area
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="minRadius"></param>
        /// <param name="maxRadius"></param>
        /// <param name="minAngle"></param>
        /// <param name="maxAngle"></param>
        /// <returns></returns>
        public static Vector2 GetRandomPointInRing(Vector2 origin, float minRadius, float maxRadius, float minAngle = 0, float maxAngle = 360)
        {
            minRadius = Mathf.Abs(minRadius);
            
            if (minRadius > maxRadius)
            {
                throw new Exception("Min radius can't be greater than max radius");
            }
            
            var angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;

            // Generate a random radius with uniform distribution over the area
            var radius = Mathf.Sqrt(Random.Range(minRadius * minRadius, maxRadius * maxRadius));
            
            var xOffset = Mathf.Cos(angle) * radius;
            var yOffset = Mathf.Sin(angle) * radius;
            
            return origin + new Vector2(xOffset, yOffset);
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
        /// Returns 32bits uid in base-16 format
        /// </summary>
        /// <returns></returns>
        public static string ShortUid()
        {
            return Random.Range(int.MinValue, int.MaxValue).ToString("x");
        }

        /// <summary>
        /// Returns lenght * 32bits uid in base-16 format (default is 64bits)
        /// </summary>
        /// <returns></returns>
        public static string Uid(int length = 2)
        {
            string uid = "";
            
            for (int i = 0; i < length; i++)
            {
                uid += Random.Range(int.MinValue, int.MaxValue).ToString("x");
            }

            return uid;
        }

        public static long GetRandomLong()
        {
            return (long)Random.Range(long.MinValue, long.MaxValue);
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
        /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            var fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin,
                from.rect.height * from.pivot.y + from.rect.yMin);
            
            var screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out var localPoint);
            
            var pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin,
                to.rect.height * to.pivot.y + to.rect.yMin);
            
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }
        
        /// <summary>
        /// Checks if an certain point is currently outside the camera's field of view
        /// </summary>
        public static bool IsInCameraFieldOfView(Vector3 position, float min = 0f, float max = 1f, Camera mainCamera = null)
        {
            if (mainCamera == null)
            {
                mainCamera = MainCam;
            }

            if (mainCamera == null)
            {
                throw new Exception("No main camera found. Pass camera as a parameter");
            }
            
            var viewportPoint = mainCamera.WorldToViewportPoint(position);
            
            if (viewportPoint.x < min || viewportPoint.x > max ||
                viewportPoint.y < min || viewportPoint.y > max ||
                viewportPoint.z < 0)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Returns string modifier for numbers. For example 1000 -> 1K, 1000000 -> 1M, 1000000000 -> 1B, etc.
        /// </summary>
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
        
        public static void RotateDirSmooth(Transform transform, Vector3 dir, float relativeToEulerY, ref float rotationVelocity, float rotationSmoothTime)
        {
            var targetRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg +
                                 relativeToEulerY;
                
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                rotationSmoothTime);
                
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        
        public static void TurnToTargetTween(Vector3 target, Transform transform, float time = 0.1f, Action done = null)
        {
            App.Core.Coroutine.Begin(TurnToTargetCoroutine(target, transform, time, done));
        }

        private static IEnumerator TurnToTargetCoroutine(Vector3 target, Transform transform, float time, Action done)
        {
            var startRotation = transform.rotation;
            var direction = (target - transform.position).normalized;
            direction.y = 0;
            var targetRotation = Quaternion.LookRotation(direction);
            
            var elapsedTime = 0f;
            
            while (elapsedTime < time)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            done?.Invoke();
        }
        
        public static void TurnToTargetInstant(Vector3 targetPoint, Transform transform)
        {
            var direction = (targetPoint - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public static void MatchForwardsSmooth(Vector3 forward, Transform transform, float time, Action done)
        {
            App.Core.Coroutine.Begin(MatchForwardsCoroutine(forward, transform, time, done));
        }
        
        private static IEnumerator MatchForwardsCoroutine(Vector3 forward, Transform transform, float time, Action done)
        {
            var start = transform.forward;
            var elapsed = 0f;
            
            var targetTransform = transform;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                targetTransform.forward = Vector3.Slerp(start, forward, elapsed / time);
                yield return null;
            }

            targetTransform.forward = forward;
            done?.Invoke();
        }
        
        // Reflection
        
        // TODO: Consider moving to ReflectionHelper
        
        public static void SetFieldWithReflection<T>(object targetObject, string fieldName, object value)
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic
                                                      | BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found");
                return;
            }

            field.SetValue(targetObject, value);
        }

        public static T GetValueWithReflection<T>(string fieldName, object targetObj)
        {
            var fieldInfo = GetFieldInfoWithReflection(fieldName, targetObj.GetType());
            
            if (fieldInfo == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found");
                return default;
            }
            
            return (T)fieldInfo.GetValue(targetObj);
        }
        
        public static FieldInfo GetFieldInfoWithReflection(string fieldName, Type type)
        {
            FieldInfo field = null;

            var parentType = type;

            while (field == null)
            {
                field = parentType.GetField(fieldName, BindingFlags.NonPublic
                                                       | BindingFlags.Instance | BindingFlags.Public);
                        
                if (parentType.BaseType == null)
                {
                    break;
                }
                        
                parentType = parentType.BaseType;
            }
            
            return field;
        }
    }
}