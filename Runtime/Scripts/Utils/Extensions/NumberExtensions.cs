using UnityEngine;

namespace DosinisSDK.Utils
{
    public static class NumberExtensions
    {
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

            // Days Hours

            return $"{time / 86400}d {time / 3600 % 24}h";
        }
        
        /// <summary>
        /// Returns time string in 00:59; 59:59; 00:59:59; 30d format.
        /// </summary>
        /// <returns></returns>
        public static string ToDurationString(this long time)
        {
            if (time < 60) // Seconds
            {
                return $"00:{time:00}";
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

            return $"{Mathf.RoundToInt(time / 86400f)}d";
        }

        // Int

        /// <summary>
        /// Returns time string in such formats: 59s; 59m 59s; 24h 59m; 30d;
        /// </summary>
        /// <returns></returns>
        public static string ToTimeString(this int time)
        {
            return ((long)time).ToTimeString();
        }

        /// <summary>
        /// Returns time string in 59; 59:59; 00:59:59; 30d format.
        /// </summary>
        /// <returns></returns>
        public static string ToDurationString(this int time)
        {
            return ((long)time).ToDurationString();
        }
        
        public static string ToPrettyString(this int number)
        {
            var str = number.ToString();

            var length = str.Length;

            if (length < 5)
            {
                return str;
            }

            var integerPartLength = length % 3;

            if (integerPartLength == 0)
                integerPartLength = 3;

            var numberOfThousands = Mathf.CeilToInt(length / 3.0f);

            var integerPart = str.Substring(0, integerPartLength);
            var fractionalPart = str.Substring(integerPartLength, 2);

            var fractional = int.Parse(fractionalPart);

            string res = fractional == 0 ? $"{integerPart}{Helper.GetStringModifier(numberOfThousands)}"
                : $"{integerPart},{fractionalPart}{Helper.GetStringModifier(numberOfThousands)}";

            return res;
        }
        
        public static string ToPrettyFracturedString(this float number)
        {
            var str = number.ToString("F2");

            var length = str.Length;
            
            if (length <= 6)
            {
                return number.ToReadableFloat();
            }
            
            str = Mathf.Round(number).ToString("F0");
            length = str.Length;
            
            var integerPartLength = length % 3;

            if (integerPartLength == 0)
                integerPartLength = 3;

            var numberOfThousands = Mathf.CeilToInt(length / 3.0f);

            var integerPart = str.Substring(0, integerPartLength);
            var fractionalPart = str.Substring(integerPartLength, 2);

            var fractional = int.Parse(fractionalPart);

            string res = fractional == 0 ? $"{integerPart}{Helper.GetStringModifier(numberOfThousands)}"
                : $"{integerPart},{fractionalPart}{Helper.GetStringModifier(numberOfThousands)}";

            return res;
        }

        // Float

        /// <summary>
        /// Returns float string in F2 format (59.00).
        /// </summary>
        /// <returns></returns>
        public static string ToF2String(this float value)
        {
            return $"{value:F2}";
        }
        
        /// <summary>
        /// Converts a float to a string, removing trailing zeros while keeping necessary decimals.
        /// </summary>
        /// <param name="value">The float value to format.</param>
        /// <returns>
        /// A string representation of the float:
        /// - If the number is whole (e.g., 1.00 → "1"), it removes the decimal part.
        /// - If the number has significant decimals (e.g., 2.20 → "2.2", 23.55 → "23.55"), it keeps them.
        /// </returns>
        public static string ToReadableFloat(this float value)
        {
            return value.ToString("0.##");
        }
        
        /// <summary>
        /// Returns time string in such formats: 59s; 59m 59s; 24h 59m; 30d;
        /// </summary>
        /// <returns></returns>
        public static string ToTimeString(this float time)
        {
            return ((long)time).ToTimeString();
        }
        
        /// <summary>
        /// Returns time string in 59; 59:59; 00:59:59; 30d format.
        /// </summary>
        /// <returns></returns>
        public static string ToDurationString(this float time)
        {
            return ((long)time).ToDurationString();
        }
    }
}
