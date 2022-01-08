using DosinisSDK.Core;

namespace DosinisSDK.RateUs
{
    [System.Serializable]
    public class RateUsData : ModuleData
    {
        public bool rated = false;
        public int starsClicked;
    }
}
