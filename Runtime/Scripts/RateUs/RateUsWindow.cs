using DosinisSDK.Core;
using DosinisSDK.UI;
using DosinisSDK.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.RateUs
{
    public class RateUsWindow : FadingWindow
    {
        [SerializeField] private Button[] rateButtons;
        [SerializeField] private Button[] lowRateButtons;

        private IRateUsManager rateUsManager;

        public override void Init(IUIManager uIManager)
        {
            base.Init(uIManager);

            rateUsManager = App.Core.GetCachedModule<IRateUsManager>();

            rateUsManager.OnInitRating += Show;

            foreach (var b in rateButtons)
            {
                b.onClick.AddListener(() =>
                {
                    rateUsManager.Rate();
                    Hide();
                });
            }

            foreach (var b in lowRateButtons)
            {
                b.onClick.AddListener(() => 
                {
                    rateUsManager.Rate(true);
                    Popup.Pop("Thanks for your rating!", 2f);
                    Hide();
                });
            }
        }
    }
}

