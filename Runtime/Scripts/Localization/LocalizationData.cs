using System;
using DosinisSDK.Core;

namespace DosinisSDK.Localization
{
    [Serializable]
    public class LocalizationData : ModuleData, IGlobalData
    {
        public string language;
    }
}
