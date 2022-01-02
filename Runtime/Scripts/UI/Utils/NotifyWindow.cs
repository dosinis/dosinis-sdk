using DosinisSDK.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI.Utils
{
    public class NotifyWindow : Window
    {
        [SerializeField] private Button ok;
        [SerializeField] private Button cancel;
        [SerializeField] private TMP_Text messageText;

        private event Action<bool> OnProceed = b => { };

        public override void Init(IUIManager uiManager)
        {
            base.Init(uiManager);
            ok.onClick.AddListener(OkClick);
            cancel.onClick.AddListener(CancelClick);
        }

        public void Show(string message, Action<bool> onProceed)
        {
            messageText.text = message;

            void CallBack(bool success)
            {
                onProceed(success);
                OnProceed -= CallBack;
            }

            OnProceed += CallBack;

            Show();
        }

        private void OkClick()
        {
            OnProceed(true);
            Hide();
        }

        private void CancelClick()
        {
            OnProceed(false);
            Hide();
        }
    }
}