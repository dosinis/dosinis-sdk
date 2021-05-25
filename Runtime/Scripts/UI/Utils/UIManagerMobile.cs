using DosinisSDK.Ads;
using DosinisSDK.Core;
using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class UIManagerMobile : UIManager
    {
        private IAdManager adManager;
        private bool adIsBeingLoaded = false;
        public override void Init(IApp app)
        {
            base.Init(app);
            adManager = app.GetCachedBehaviourModule<IAdManager>();
        }
        public void ShowRewardedAdWindow(Action<bool> callback)
        {
            var messageWindow = GetWindow<MessageWindow>();
            messageWindow.Show("One moment!", "Ad is being loaded...");
            messageWindow.SetCloseButtonEnabled(false);

            adManager.ShowRewardedAd(success =>
            {
                adIsBeingLoaded = true;

                if (success)
                {
                    adIsBeingLoaded = false;
                    messageWindow.Hide();
                    messageWindow.SetCloseButtonEnabled(true);
                }

                callback(success);
            });

            StartCoroutine(AdWindowButtonCoroutine(messageWindow));
        }

        private IEnumerator AdWindowButtonCoroutine(MessageWindow messageWindow)
        {
            float timer = 10f;
            while (timer > 0)
            {
                timer -= Time.deltaTime;

                if (adIsBeingLoaded == false)
                {
                    yield break;
                }

                yield return null;
            }

            if (adIsBeingLoaded)
            {
                messageWindow.SetupBody("OOPS!", "Something went wrong");
                messageWindow.SetCloseButtonEnabled(true);
                adIsBeingLoaded = false;
            }
        }
    }
}


