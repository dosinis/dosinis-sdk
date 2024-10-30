using System.IO;
using DosinisSDK.Audio;
using DosinisSDK.Core;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DosinisSDK.Utils
{
    public static class Extensions
    {
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

        // Renderer

        public static void InstantiateMaterials(this Renderer rend)
        {
            Material[] materials = rend.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = Object.Instantiate(materials[i]);
            }

            rend.materials = materials;
        }
        
        public static void SetInstanceMaterialColor(this Renderer rend, Color color)
        {
            Material[] materials = rend.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                var materialInstance = Object.Instantiate(materials[i]);

                materialInstance.color = color;

                materials[i] = materialInstance;
            }

            rend.materials = materials;
        }

        public static void SetInstanceMaterialTexture(this Renderer rend, Texture tex)
        {
            Material[] materials = rend.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                var materialInstance = Object.Instantiate(materials[i]);

                materialInstance.mainTexture = tex;

                materials[i] = materialInstance;
            }

            rend.materials = materials;
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

        public static Vector3 GetRandomPointAround(this Transform transform, float maxDist, float minDist = 0f)
        {
            var randomRadius = Random.Range(minDist, maxDist);
            var randomAngle = Random.Range(0f, 2f * Mathf.PI);

            var x = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
            var z = transform.position.z + randomRadius * Mathf.Sin(randomAngle);

            var randomPoint = new Vector3(x, transform.position.y, z);
            
            return randomPoint;
        }
        
        public static void SetLayerRecursively(this Transform obj, int layer)
        {
            obj.gameObject.layer = layer;
            
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Returns the corrected Euler angles for the transform (same as displayed in the inspector).
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector3 GetCorrectEulerAngles(this Transform transform)
        {
            var x = Helper.TranslateEulerAngle(transform.eulerAngles.x);
            var y = Helper.TranslateEulerAngle(transform.eulerAngles.y);
            var z = Helper.TranslateEulerAngle(transform.eulerAngles.z);
            
            return new Vector3(x, y, z);
        }
        
        /// <summary>
        /// Returns the corrected local Euler angles for the transform (same as displayed in the inspector).
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector3 GetCorrectLocalEulerAngles(this Transform transform)
        {
            var x = Helper.TranslateEulerAngle(transform.localEulerAngles.x);
            var y = Helper.TranslateEulerAngle(transform.localEulerAngles.y);
            var z = Helper.TranslateEulerAngle(transform.localEulerAngles.z);
            
            return new Vector3(x, y, z);
        }

        public static void ResetTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        // Color

        public static Color SetAlpha(ref this Color color, float alpha)
        {
            color = new Color(color.r, color.g, color.b, alpha);

            return color;
        }

        // GameObject
        
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static bool IsInScene(this GameObject obj)
        {
            return string.IsNullOrEmpty(obj.scene.name) == false;
        }
        
        // Audio

        /// <summary>
        /// Plays a sound effect if the AudioManager is available.
        /// </summary>
        public static void PlayOneShotSafe(this AudioClip clip, float volume = 1)
        {
            if (App.Core.TryGetModule(out IAudioManager audio))
            {
                audio.PlayOneShot(clip, volume);
            }
        }

        public static void PlayOneShotSafeAtPoint(this AudioClip clip, Vector3 point, float minDistance = 3f, float maxDistance = 100f, float volume = 1)
        {
            if (App.Core.TryGetModule(out IAudioManager audio))
            {
                audio.PlayAtPoint(clip, point, minDistance, maxDistance, volume);
            }
        }
        
        // LayerMask
        
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
        
        // Rigidbody
        
        public static void Translate(this Rigidbody rb, Vector3 translation, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                rb.MovePosition(rb.position + rb.transform.TransformDirection(translation));
            }
            else
            {
                rb.MovePosition(rb.position + translation);
            }
        }

        public static void Translate(this Rigidbody rb, float x, float y, float z, Space relativeTo = Space.Self)
        {
            rb.Translate(new Vector3(x, y, z), relativeTo);
        }
    }
}