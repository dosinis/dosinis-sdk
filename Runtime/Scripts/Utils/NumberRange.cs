namespace DosinisSDK.Utils
{
    [System.Serializable]
    public class IntRange
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
    }

    [System.Serializable]
    public class FloatRange
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
        public float GetRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
