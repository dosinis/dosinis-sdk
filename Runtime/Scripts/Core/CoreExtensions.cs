using System;
using System.Collections;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

namespace DosinisSDK.Core
{
    public static class CoreExtensions
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
        
        public static T First<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            return array[0];
        }
        
        public static T First<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[0];
        }

        public static T Last<T>(this T[] array)
        {
            if (array.Length == 0)
                return default;

            return array[^1];
        }

        public static T Last<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[^1];
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
        
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue valueToAdd)
        {
            if (dictionary.TryGetValue(key, out var value)) 
                return value;

            value = valueToAdd;
            dictionary[key] = value;
            
            return value;
        }

        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
