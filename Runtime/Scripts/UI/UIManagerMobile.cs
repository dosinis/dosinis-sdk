using DosinisSDK.Ads;
using DosinisSDK.Core;
using DosinisSDK.UI.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class UIManagerMobile : UIManager
    {
        private IAdManager adManager;
        private bool adIsBeingLoaded = false;

        public override void Init(ISceneManager sceneManager)
        {
            base.Init(sceneManager);
            adManager = App.Core.GetCachedModule<IAdManager>();
        }

        public void ShowRewardedAdWindow(string placement, Action<bool> callback)
        {
            var messageWindow = GetWindow<MessageWindow>();

            adIsBeingLoaded = true;

            adManager.ShowRewardedAd(placement, success =>
            {
                if (success)
                {
                    adIsBeingLoaded = false;

                    if (messageWindow.isActiveAndEnabled)
                        messageWindow.Hide();
                }

                callback(success);
            });
            
            StartCoroutine(AdWindowButtonCoroutine(messageWindow));
        }

        private IEnumerator AdWindowButtonCoroutine(MessageWindow messageWindow)
        {
            yield return new WaitForSeconds(0.1f);

            float timer = 10f;

            if (adIsBeingLoaded == false)
                yield break;

            messageWindow.Show("One moment!", "Ad is loading...");
            messageWindow.SetCloseButtonEnabled(false);

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                
                if (adIsBeingLoaded == false)
                {
                    messageWindow.Hide();
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


