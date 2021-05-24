using System.Collections;
using System.Collections.Generic;

namespace DosinisSDK.Utils
{
    public static class Extensions
    {
        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
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
    }

}