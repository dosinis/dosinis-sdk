using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.RateUs
{
    public class RateUsManager : Module, IRateUsManager
    {
        private RateUsData data;
        public bool IsRated => data.rated;

        public override void Init(IApp app)
        {
            var dataManager = app.GetCachedModule<IDataManager>();
            data = dataManager.LoadAndRegisterData<RateUsData>();
        }

        public void Rate()
        {
            Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
            data.rated = true;
        }
    }
}

