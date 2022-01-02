using DosinisSDK.Core;
using TMPro;
using UnityEngine;

namespace DosinisSDK.UI.Utils
{
    public class Popup : Window
    {
        [SerializeField] private TMP_Text messageText;

        private void Show(string message, float duration = 1f)
        {
            messageText.text = message;

            Show();

            App.Core.Timer.Delay(duration, () =>
            {
                Hide();
            });
        }

        public static void Pop(string message, float duration = 1f)
        {
           // App.Core.SceneManager.GetManaged<IUIManager>().GetWindow<Popup>().Show(message, duration);
        }
    }
}
