using TMPro;
using UnityEngine;

namespace DosinisSDK.UI.Utils
{
    public class MessageWindow : AnimatedWindow
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;

        public void Show(string title, string message)
        {
            SetupBody(title, message);

            Show();
        }

        public void SetupBody(string title, string message)
        {
            titleText.text = title;
            messageText.text = message;
        }

        public void SetCloseButtonEnabled(bool value)
        {
            //closeButton.gameObject.SetActive(value);
        }
    }
}


