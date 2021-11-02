using DosinisSDK.Core;
using DosinisSDK.UI;
using UnityEngine;

namespace DosinisSDK.RateUs
{
    public class RateUsWindow : FadingWindow
    {
        [SerializeField] private BouncyButton[] rateButtons;
        [SerializeField] private BouncyButton[] lowRateButtons;

        private IRateUsManager rateUsManager;

        public override void Init(IUIManager uIManager)
        {
            base.Init(uIManager);

            rateUsManager = App.Core.GetCachedModule<IRateUsManager>();

            foreach (var b in rateButtons)
            {
                b.onClick.AddListener(() =>
                {
                    rateUsManager.Rate();
                });
            }

            foreach (var b in lowRateButtons)
            {
                b.onClick.AddListener(() => 
                {
                    Popup.Pop("Thanks for your rating!", 2f);
                });
            }

        }
    }
}

