using DosinisSDK.Core;
using System;
using UnityEngine;

namespace DosinisSDK.RateUs
{
    public class RateUsManager : Module, IRateUsManager
    {
        private RateUsData data;

        public bool IsRated => data.rated;
        public event Action OnInitRating = () => { };

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

        public void InitRating()
        {
            if (IsRated == false)
            {
                OnInitRating();
            }
        }
    }
}

