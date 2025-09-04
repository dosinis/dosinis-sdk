namespace DosinisSDK.Utils
{
    [System.Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        
        /// <summary>
        /// Returns Random between min and max (both inclusive)
        /// </summary>
        /// <returns></returns>
        public int GetRandom()
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        public int Average()
        {
            return (min + max) / 2;
        }
        
        public override string ToString()
        {
            return $"{min}-{max}";
        }
    }

    [System.Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        
        /// <summary>
        /// Returns Random between min and max (both inclusive)
        /// </summary>
        /// <returns></returns>
        public readonly float GetRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
        
        public readonly float Average()
        {
            return (min + max) / 2f;
        }
        
        public override string ToString()
        {
            return $"[{min}, {max}]";
        }
    }
}
