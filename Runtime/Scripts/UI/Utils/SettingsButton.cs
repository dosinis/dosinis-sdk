using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI.Utils
{
    [RequireComponent(typeof(Button))]
    public class SettingsButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                App.Core.GetCachedModule<IUIManager>().GetWindow<SettingsWindow>().Show();
            });
        }
    }

}
