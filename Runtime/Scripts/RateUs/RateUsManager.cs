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

        protected override void OnInit(IApp app)
        {
            var dataManager = app.GetCachedModule<IDataManager>();
            data = dataManager.RetrieveOrCreateData<RateUsData>();
        }

        public void Rate(bool dummyRate = false)
        {
            if (dummyRate == false)
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

