namespace DosinisSDK.Utils
{
    [System.Serializable]
    public class IntRange
    {
        public int min;
        public int max;

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
