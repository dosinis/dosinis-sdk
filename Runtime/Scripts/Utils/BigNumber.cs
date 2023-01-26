using System;
using System.Numerics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace DosinisSDK.Utils
{
    [Serializable]
    public struct BigNumber : ISerializationCallbackReceiver
    {
        [JsonProperty]
        [SerializeField] private string stringValue;

        private BigInteger? bigValue;

        private static readonly BigInteger MAX_FLOAT = new BigInteger(float.MaxValue);
        private static readonly BigInteger MAX_DOULBE = new BigInteger(double.MaxValue);

        // Properties

        [JsonIgnore]
        public bool FitsInFloat => Value < MAX_FLOAT;
        [JsonIgnore]
        public bool FitsInDouble => Value < MAX_DOULBE;
        [JsonIgnore]
        public bool FitsInLong => Value < long.MaxValue;

        [JsonIgnore]
        public BigInteger Value
        {
            get
            {
                if (bigValue.HasValue == false)
                {
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        bigValue = 0;
                        stringValue = bigValue.Value.ToString();
                        return 0;
                    }

                    if (BigInteger.TryParse(stringValue, out BigInteger v))
                    {
                        bigValue = v;
                    }
                    else
                    {
                        Debug.LogError($"Cannot parse big int {stringValue}");
                        bigValue = 0;
                    }
                }
                return bigValue.Value;
            }
            set
            {
                if (bigValue != value)
                {
                    bigValue = value;
#if UNITY_EDITOR
                    stringValue = value.ToString();
#endif
                }
            }
        }

        // Constructors

        public BigNumber(string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
            {
                numberString = "0";
            }

            if (BigInteger.TryParse(numberString, out BigInteger v) == false)
            {
                Debug.LogError($"Cannot parse big int {numberString}");
                numberString = "0";
            }

            bigValue = v;
            stringValue = numberString;
        }

        public BigNumber(BigInteger number)
        {
            stringValue = null;
            bigValue = number;
        }

        public BigNumber(int number)
        {
            stringValue = null;
            bigValue = number;
        }

        // Equality

        public override bool Equals(object obj)
        {
            if (!(obj is BigNumber))
            {
                return false;
            }

            var number = (BigNumber)obj;
            return number.Value == Value;
        }

        public override int GetHashCode()
        {
            return 1101458105 + Value.GetHashCode();
        }

        // Compression

        public static BigNumber FromCompressedInt(int compressedInt, int presicionDigitsCount = 6)
        {
            if (compressedInt < BigInteger.Pow(10, presicionDigitsCount))
            {
                return compressedInt;
            }

            string numString = compressedInt.ToString();

            int.TryParse(numString.Substring(0, numString.Length - presicionDigitsCount), out int zeroesCount);
            int.TryParse(numString.Substring(numString.Length - presicionDigitsCount, presicionDigitsCount), out int num);

            return num * BigInteger.Pow(10, zeroesCount);
        }

        public int ConvertToCompressedInt(int presicionDigitsCount = 6)
        {
            return Value.ToCompressedInt(presicionDigitsCount);
        }

        // String

        public override string ToString()
        {
            return Value.ToString();
        }

        // Operators

        public static implicit operator BigNumber(int value)
        {
            return new BigNumber(value);
        }

        public static implicit operator BigNumber(uint value)
        {
            return new BigNumber(value);
        }

        public static implicit operator BigNumber(BigInteger value)
        {
            return new BigNumber(value);
        }

        public static implicit operator BigNumber(string value)
        {
            return new BigNumber(value);
        }

        public static BigNumber operator +(BigNumber v1, BigInteger v2)
        {
            v1.Value += v2;
            return v1;
        }

        public static BigNumber operator +(BigNumber v1, BigNumber v2)
        {
            return v1.Value += v2.Value;
        }

        public static BigNumber operator -(BigNumber v1, BigInteger v2)
        {
            v1.Value -= v2;
            return v1;
        }

        public static BigNumber operator -(BigNumber v1, BigNumber v2)
        {
            v1.Value -= v2.Value;
            return v1;
        }

        public static bool operator >(BigNumber v1, BigNumber v2)
        {
            return v1.Value > v2.Value;
        }

        public static bool operator <(BigNumber v1, BigNumber v2)
        {
            return v1.Value < v2.Value;
        }

        public static bool operator >=(BigNumber v1, BigNumber v2)
        {
            return v1.Value >= v2.Value;
        }

        public static bool operator <=(BigNumber v1, BigNumber v2)
        {
            return v1.Value <= v2.Value;
        }

        public static bool operator ==(BigNumber v1, BigNumber v2)
        {
            return v1.Value == v2.Value;
        }

        public static bool operator !=(BigNumber v1, BigNumber v2)
        {
            return v1.Value != v2.Value;
        }

        public static BigNumber operator *(BigNumber v1, BigNumber v2)
        {
            v1.Value *= v2.Value;
            return v1;
        }

        public static BigNumber operator /(BigNumber v1, BigNumber v2)
        {
            v1.Value /= v2.Value;
            return v1;
        }

        // Serialization

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            if (bigValue.HasValue)
            {
                stringValue = bigValue.ToString();
            }

            if (string.IsNullOrEmpty(stringValue))
            {
                stringValue = "0";
            }
        }

        public void OnBeforeSerialize()
        {
            if (bigValue.HasValue)
            {
                stringValue = bigValue.ToString();
            }

            if (string.IsNullOrEmpty(stringValue))
            {
                stringValue = "0";
            }
        }

        public void OnAfterDeserialize()
        {
            if (BigInteger.TryParse(stringValue, out BigInteger res))
            {
                bigValue = res;
            }
            else
            {
                bigValue = 0;
            }
        }

        // Utils

        /// <summary>
        /// Returns bigger one out of the two
        /// </summary>
        /// <returns></returns>
        public static BigNumber Max(BigNumber v1, BigNumber v2)
        {
            if (v1 > v2)
                return v1;

            return v2;
        }

        /// <summary>
        /// Returns smaller one out of the two
        /// </summary>
        /// <returns></returns>
        public static BigNumber Min(BigNumber v1, BigNumber v2)
        {
            if (v1 > v2)
                return v2;

            return v1;
        }

        /// <summary>
        /// Return Random number between two
        /// </summary>
        /// <returns></returns>
        public static BigNumber Random(BigNumber v1, BigNumber v2)
        {
            if (v1.FitsInLong && (v2 + 1).FitsInLong)
            {
                return (BigInteger)UnityEngine.Random.Range((long)v1.Value, (long)v2.Value + 1);
            }
            
            return Lerp(v1, v2, UnityEngine.Random.Range(0f, 1f));
        }
        
        /// <summary>
        /// Linear interpolation utility
        /// </summary>
        /// <param name="v1">from</param>
        /// <param name="v2">to</param>
        /// <param name="t">normalizedTime</param>
        /// <returns></returns>
        public static BigNumber Lerp(BigNumber v1, BigNumber v2, float t)
        {
            return v1.MultiplyByFloat(1f - t) + v2.MultiplyByFloat(t);
        }
        
        public static BigNumber Zero => 0;
    }

    public static class BigNumberExtensions
    {
        public static int ToCompressedInt(this BigInteger number, int precisionDigitsCount = 6)
        {
            string numberString = number.ToString();

            int zeroesCount = numberString.Length;

            if (zeroesCount < precisionDigitsCount)
            {
                return (int)number;
            }

            string roundedNumberString = numberString.Substring(0, precisionDigitsCount);

            string zeroesAmountString = (zeroesCount - precisionDigitsCount).ToString();

            if (int.TryParse($"{zeroesAmountString}{roundedNumberString}", out int result))
            {
                return result;
            }

            return 0;
        }

        private static string GetStringModifier(int numberOfThousands)
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

        public static string ToPrettyFracturedString(this BigNumber number, int symbolsAfterComma = 1)
        {
            var value = number.Value;
            
            var str = value.ToString();

            var length = str.Length;

            if (length < 4)
            {
                return str;
            }

            var integerPartLength = length % 3;

            if (integerPartLength == 0)
                integerPartLength = 3;

            var numberOfThousands = Mathf.CeilToInt(length / 3.0f);

            var integerPart = str.Substring(0, integerPartLength);
            var fractionalPart = str.Substring(integerPartLength, symbolsAfterComma);

            var fractional = int.Parse(fractionalPart);

            string res = fractional == 0 ? $"{integerPart}{GetStringModifier(numberOfThousands)}"
                : $"{integerPart},{fractionalPart}{GetStringModifier(numberOfThousands)}";
            
            return res;
        }

        public static string ToPrettyString(this BigNumber number)
        {
            var value = number.Value;
            
            var str = value.ToString();

            var length = str.Length;

            if (length < 4)
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

            string res = fractional == 0 ? $"{integerPart}{GetStringModifier(numberOfThousands)}"
                : $"{integerPart},{fractionalPart}{GetStringModifier(numberOfThousands)}";

            return res;
        }

        public static double PreciseDivideDouble(this BigNumber number, BigNumber divider, int precision = 1000)
        {
            return (double)(precision * number.Value / divider.Value) / precision;
        }
        
        public static float PreciseDivideFloat(this BigNumber number, BigNumber divider, int precision = 1000)
        {
            return (float)(precision * number.Value / divider.Value) / precision;
        }
        
        public static BigNumber MultiplyByFloat(this BigNumber bigNum, float value, int precision = 1000)
        {
            var bigInt = bigNum.Value;

            if (value == 0)
                return 0;

            if (bigInt < 10000)
            {
                float tmpVal = (long)bigInt;

                tmpVal *= value;

                bigInt = (long)Mathf.Ceil(tmpVal);

                return bigInt;
            }

            var multiplierPercent = (long)(value * precision);
            bigInt *= multiplierPercent;

            return bigInt / precision;
        }

        public static BigNumber MultiplyByDouble(this BigNumber bigNum, double value, int precision = 1000)
        {
            var bigInt = bigNum.Value;

            if (value == 0)
                return 0;

            if (bigInt < 10000)
            {
                double tmpVal = (long)bigInt;

                tmpVal *= value;

                bigInt = (long)Math.Ceiling(tmpVal);

                return bigInt;
            }
            
            var multiplierPercent = (long)(value * precision);
            bigInt *= multiplierPercent;
            return bigInt / precision;
        }

        public static BigNumber DivideByFloat(this BigNumber bigNum, float value, int precision = 1000)
        {
            var big = bigNum * precision;
            var division = new BigInteger(value * precision);

            if (division == 0)
                division = 1;

            big /= division;

            return big;
        }

        public static BigNumber DivideByDouble(this BigNumber bigNum, double value, int precision = 1000)
        {
            var big = bigNum * precision;
            var division = new BigInteger(value * precision);

            if (division == 0)
                division = 1;

            big /= division;

            return big;
        }

        public static BigNumber Pow(this BigNumber bigNum, int power)
        {
            return BigInteger.Pow(bigNum.Value, power);
        }

        public static BigNumber BigNumberPow(this float number, long power)
        {
            if (power == 0)
                return 1;

            BigNumber bigNum;
            float small = number;
            long index;

            for (long i = 0; i < power - 1; i++)
            {
                if (small > 1000)
                {
                    index = i;
                    bigNum = (int)small;

                    while (index < power - 1)
                    {
                        bigNum = bigNum.MultiplyByFloat(number);
                        index++;
                    }

                    return bigNum;
                }

                small *= number;
            }

            bigNum = (int)small;

            return bigNum;
        }
    }
}