using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
        public static T Random<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[UnityRandom.Range(0, list.Count)];
        }

        public static T CachedRandom<T>(this IReadOnlyList<T> list, HashSet<int> set)
        {
            if (list == null || list.Count == 0)
                return default;

            var available = new List<T>();

            foreach (var item in list)
            {
                if (!set.Contains(item.GetHashCode()))
                {
                    available.Add(item);
                }
            }

            if (available.Count == 0)
            {
                foreach (var item in list)
                {
                    set.Remove(item.GetHashCode());
                }

                available.AddRange(list);
            }

            var selected = available[UnityRandom.Range(0, available.Count)];
            set.Add(selected.GetHashCode());

            return selected;
        }

        public static T[] RandomRange<T>(this T[] array, int amount, bool canRepeat = true)
        {
            if (array.Length == 0)
            {
                Debug.LogWarning("Tried to get a random range from an empty list");
                return Array.Empty<T>();
            }
            
            var rangeArray = new T[amount];
            
            int exitCount = 0;
            
            for (int i = 0; i < amount; i++)
            {
                var random = array.Random();
                
                if (canRepeat)
                {
                    rangeArray[i] = random;
                }
                else
                {
                    while (rangeArray.Contains(random) && exitCount < 100)
                    {
                        random = array.Random();
                        exitCount++;
                    }

                    if (exitCount >= 100)
                    {
                        Debug.LogWarning("Tried to get a random unique value from" +
                                         " an array 100 times without success, breaking loop");
                    }
                    
                    rangeArray[i] = random;
                }
            }
            
            return rangeArray;
        }
        
        public static List<T> RandomRange<T>(this List<T> list, int amount, bool canRepeat = true)
        {
            return ((IReadOnlyList<T>)list).RandomRange(amount, canRepeat);
        }
        
        public static List<T> RandomRange<T>(this IReadOnlyList<T> list, int amount, bool canRepeat = true)
        {
            if (list.Count == 0)
            {
                Debug.LogWarning("Tried to get a random range from an empty list");
                return new List<T>();
            }
            
            var rangeList = new List<T>();
            
            int exitCount = 0;
            
            for (int i = 0; i < amount; i++)
            {
                var random = list.Random();
                
                if (canRepeat)
                {
                    rangeList.Add(random);
                }
                else
                {
                    while (rangeList.Contains(random) && exitCount < 100)
                    {
                        random = list.Random();
                        exitCount++;
                    }

                    if (exitCount >= 100)
                    {
                        Debug.LogWarning("Tried to get a random unique value from" +
                                         " a list 100 times without success, breaking loop");
                    }
                    
                    rangeList.Add(random);
                }
            }
            
            return rangeList;
        }
        
        public static T RandomFromRange<T>(this List<T> list, int startIndex, int endIndex)
        {
            if (list.Count == 0)
                return default;

            return list[UnityRandom.Range(startIndex, endIndex)];
        }
        
        public static T RandomFromRange<T>(this T[] array, int startIndex, int endIndex)
        {
            if (array.Length == 0)
                return default;

            return array[UnityRandom.Range(startIndex, endIndex)];
        }
        
        public static T RandomFromRange<T>(this IReadOnlyList<T> list, int startIndex, int endIndex)
        {
            if (list.Count == 0)
                return default;

            return list[UnityRandom.Range(startIndex, endIndex)];
        }

        public static int RandomIndex<T>(this T[] array)
        {
            if (array.Length == 0)
                return 0;

            return UnityRandom.Range(0, array.Length);
        }
        
        public static int RandomIndex<T>(this List<T> list)
        {
            return ((IReadOnlyList<T>)list).RandomIndex();
        }
        
        public static int RandomIndex<T>(this IReadOnlyList<T> list)
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
        
        public static T First<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[0];
        }
        
        public static T Last<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[^1];
        }
        
        public static bool Contains<T>(this T[] array, T value)
        {
            return Array.Exists(array, x => x != null && x.Equals(value));
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

        public static T Find<T>(this IEnumerable<T> collection, Predicate<T> match)
        {
            foreach (var element in collection)
            {
                if (match(element))
                {
                    return element;
                }
            }

            return default;
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
                int randomIndex = UnityRandom.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        public static void ClearValues<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default;
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

        public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary[key] = value;
        }

        public static void AddOrSetList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue item)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(item);
            }
            else
            {
                dictionary.Add(key, new List<TValue> {item});
            }
        }

        public static void Increment<TKey>(this IDictionary<TKey, int> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        /// <summary>
        /// Removes all entries from the dictionary that match the conditions defined by the specified predicate.
        /// </summary>
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TValue> match)
        {
            var keysToRemove = new List<TKey>();
            
            foreach (var key in dictionary.Keys)
            {
                if (match(dictionary[key]))
                {
                    keysToRemove.Add(key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                dictionary.Remove(key);
            }
        }
    }
}
